using Tienda.Application.Abstractions;
using Tienda.Domain.Entities;
using Tienda.Domain.Exceptions;

namespace Tienda.Application.Catalogos;

/// <summary>
/// Ejecuta operaciones de aplicación para el catálogo de productos.
/// </summary>
public sealed class ProductoService(IProductoRepository productos, IUnitOfWork unidadDeTrabajo)
{
    /// <summary>
    /// Registra un producto si su SKU aún no está utilizado.
    /// </summary>
    /// <param name="sku">SKU del producto.</param>
    /// <param name="nombre">Nombre del producto.</param>
    /// <param name="precioUnitario">Precio vigente definido en el servidor.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El producto creado.</returns>
    /// <exception cref="DomainRuleException">Si el SKU ya existe o los datos incumplen reglas del dominio.</exception>
    public async Task<Producto> CrearAsync(
        string sku,
        string nombre,
        decimal precioUnitario,
        CancellationToken cancellationToken = default)
    {
        var skuNormalizado = sku?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(skuNormalizado))
        {
            throw new DomainRuleException("El SKU del producto es obligatorio.");
        }

        if (await productos.ExisteSkuAsync(skuNormalizado, cancellationToken: cancellationToken))
        {
            throw new DomainRuleException("Ya existe un producto con el SKU indicado.");
        }

        var producto = new Producto(Guid.NewGuid(), skuNormalizado, nombre, precioUnitario);
        await productos.AgregarAsync(producto, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        return producto;
    }

    /// <summary>
    /// Desactiva un producto sin eliminarlo ni borrar su historial.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <exception cref="DomainRuleException">Si el producto no existe.</exception>
    public async Task DesactivarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var producto = await productos.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new DomainRuleException("El producto indicado no existe.");

        producto.Desactivar();
        await productos.ActualizarAsync(producto, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina un producto únicamente si no tiene compras ni ventas asociadas.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <exception cref="DomainRuleException">Si el producto no existe o tiene movimientos.</exception>
    public async Task EliminarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var transaccion = await unidadDeTrabajo.IniciarTransaccionAsync(cancellationToken);
        var producto = await productos.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new DomainRuleException("El producto indicado no existe.");

        if (await productos.TieneMovimientosAsync(id, cancellationToken))
        {
            throw new DomainRuleException("No se puede eliminar un producto con compras o ventas asociadas; desactívelo.");
        }

        await productos.EliminarAsync(producto, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        await transaccion.ConfirmarAsync(cancellationToken);
    }
}
