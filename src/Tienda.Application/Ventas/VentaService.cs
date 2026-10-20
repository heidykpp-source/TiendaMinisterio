using Tienda.Application.Abstractions;
using Tienda.Domain.Entities;
using Tienda.Domain.Exceptions;

namespace Tienda.Application.Ventas;

/// <summary>
/// Ejecuta el caso de uso transaccional de registro de ventas.
/// </summary>
public sealed class VentaService(
    IClienteRepository clientes,
    IProductoRepository productos,
    IVentaRepository ventas,
    IUnitOfWork unidadDeTrabajo,
    TimeProvider timeProvider)
{
    /// <summary>
    /// Valida la venta completa, descuenta existencias y persiste venta y stock atómicamente.
    /// </summary>
    /// <param name="clienteId">Identificador del cliente.</param>
    /// <param name="lineasSolicitadas">Productos y cantidades; los precios siempre se obtienen del servidor.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>La venta registrada con precios históricos determinados por el servidor.</returns>
    /// <exception cref="DomainRuleException">Si el cliente, alguna línea o el stock no permiten registrar la venta.</exception>
    public async Task<Venta> RegistrarAsync(
        Guid clienteId,
        IEnumerable<LineaVentaSolicitada> lineasSolicitadas,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lineasSolicitadas);
        var lineas = lineasSolicitadas.ToArray();
        if (lineas.Length == 0)
        {
            throw new DomainRuleException("La venta debe tener al menos una línea.");
        }

        foreach (var linea in lineas)
        {
            if (linea is null)
            {
                throw new DomainRuleException("La venta contiene una línea inválida.");
            }

            if (linea.ProductoId == Guid.Empty)
            {
                throw new DomainRuleException("El producto de la línea de venta es obligatorio.");
            }

            if (linea.Cantidad <= 0)
            {
                throw new DomainRuleException("La cantidad de la línea de venta debe ser mayor a cero.");
            }
        }

        var cantidadesPorProducto = lineas
            .GroupBy(linea => linea.ProductoId)
            .ToDictionary(grupo => grupo.Key, grupo => grupo.Sum(linea => linea.Cantidad));

        await using var transaccion = await unidadDeTrabajo.IniciarTransaccionAsync(cancellationToken);

        var cliente = await clientes.ObtenerPorIdAsync(clienteId, cancellationToken)
            ?? throw new DomainRuleException("El cliente indicado no existe.");
        if (!cliente.Activo)
        {
            throw new DomainRuleException("No se puede registrar una venta para un cliente inactivo.");
        }

        var productosEncontrados = await productos.ObtenerPorIdsAsync(
            lineas.Select(linea => linea.ProductoId).Distinct().ToArray(),
            cancellationToken);
        var productosPorId = productosEncontrados.ToDictionary(producto => producto.Id);

        foreach (var (idProducto, cantidadSolicitada) in cantidadesPorProducto)
        {
            if (!productosPorId.TryGetValue(idProducto, out var producto))
            {
                throw new DomainRuleException("Uno de los productos indicados no existe.");
            }

            if (!producto.Activo)
            {
                throw new DomainRuleException("No se puede vender un producto inactivo.");
            }

            if (cantidadSolicitada > producto.Stock)
            {
                throw new DomainRuleException("No hay stock suficiente para realizar la venta.");
            }
        }

        var lineasVenta = lineas
            .Select(linea => new VentaLinea(
                linea.ProductoId,
                linea.Cantidad,
                productosPorId[linea.ProductoId].PrecioUnitario))
            .ToArray();
        var venta = new Venta(
            Guid.NewGuid(),
            clienteId,
            lineasVenta,
            timeProvider.GetUtcNow().UtcDateTime);

        foreach (var (productoId, cantidad) in cantidadesPorProducto)
        {
            var producto = productosPorId[productoId];
            producto.RegistrarVenta(cantidad);
            await productos.ActualizarAsync(producto, cancellationToken);
        }

        await ventas.AgregarAsync(venta, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        await transaccion.ConfirmarAsync(cancellationToken);
        return venta;
    }
}
