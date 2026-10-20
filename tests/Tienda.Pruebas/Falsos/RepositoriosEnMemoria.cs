using Tienda.Application.Abstractions;
using Tienda.Domain.Entities;

namespace Tienda.Pruebas.Falsos;

public sealed class ClienteRepositoryFalso : IClienteRepository
{
    private readonly Dictionary<Guid, Cliente> clientesPorId = new();

    public List<Cliente> Agregados { get; } = new();

    public void Sembrar(params Cliente[] clientes)
    {
        foreach (var cliente in clientes)
        {
            clientesPorId[cliente.Id] = cliente;
        }
    }

    public Task<bool> ExisteDocumentoAsync(string documento, Guid? excluirClienteId = null, CancellationToken cancellationToken = default)
    {
        var existe = clientesPorId.Values.Any(c =>
            string.Equals(c.Documento.Valor, documento, StringComparison.OrdinalIgnoreCase)
            && (!excluirClienteId.HasValue || c.Id != excluirClienteId.Value));

        return Task.FromResult(existe);
    }

    public Task<Cliente?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        clientesPorId.TryGetValue(id, out var cliente);
        return Task.FromResult(cliente);
    }

    public Task AgregarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        Agregados.Add(cliente);
        clientesPorId[cliente.Id] = cliente;
        return Task.CompletedTask;
    }
}

public sealed class ProductoRepositoryFalso : IProductoRepository
{
    private readonly Dictionary<Guid, Producto> productosPorId = new();

    public HashSet<Guid> ProductosConMovimientos { get; } = new();
    public List<Producto> Agregados { get; } = new();
    public List<Producto> Actualizados { get; } = new();
    public List<Producto> Eliminados { get; } = new();

    public void Sembrar(params Producto[] productos)
    {
        foreach (var producto in productos)
        {
            productosPorId[producto.Id] = producto;
        }
    }

    public Task<bool> ExisteSkuAsync(string sku, Guid? excluirProductoId = null, CancellationToken cancellationToken = default)
    {
        var existe = productosPorId.Values.Any(p =>
            string.Equals(p.Sku, sku, StringComparison.OrdinalIgnoreCase)
            && (!excluirProductoId.HasValue || p.Id != excluirProductoId.Value));

        return Task.FromResult(existe);
    }

    public Task<Producto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        productosPorId.TryGetValue(id, out var producto);
        return Task.FromResult(producto);
    }

    public Task<bool> TieneMovimientosAsync(Guid productoId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ProductosConMovimientos.Contains(productoId));
    }

    public Task<IReadOnlyList<Producto>> ObtenerPorIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        var encontrados = ids.Where(productosPorId.ContainsKey).Select(id => productosPorId[id]).ToArray();
        return Task.FromResult((IReadOnlyList<Producto>)encontrados);
    }

    public Task AgregarAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        Agregados.Add(producto);
        productosPorId[producto.Id] = producto;
        return Task.CompletedTask;
    }

    public Task ActualizarAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        Actualizados.Add(producto);
        productosPorId[producto.Id] = producto;
        return Task.CompletedTask;
    }

    public Task EliminarAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        Eliminados.Add(producto);
        productosPorId.Remove(producto.Id);
        return Task.CompletedTask;
    }
}

public sealed class ProveedorRepositoryFalso : IProveedorRepository
{
    private readonly Dictionary<Guid, Proveedor> proveedoresPorId = new();

    public List<Proveedor> Agregados { get; } = new();

    public Task<bool> ExisteNitAsync(string nit, Guid? excluirProveedorId = null, CancellationToken cancellationToken = default)
    {
        var existe = proveedoresPorId.Values.Any(p =>
            string.Equals(p.Nit.Valor, nit, StringComparison.OrdinalIgnoreCase)
            && (!excluirProveedorId.HasValue || p.Id != excluirProveedorId.Value));

        return Task.FromResult(existe);
    }

    public Task AgregarAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
    {
        Agregados.Add(proveedor);
        proveedoresPorId[proveedor.Id] = proveedor;
        return Task.CompletedTask;
    }
}

public sealed class VentaRepositoryFalso : IVentaRepository
{
    public List<Venta> Agregadas { get; } = new();

    public Task AgregarAsync(Venta venta, CancellationToken cancellationToken = default)
    {
        Agregadas.Add(venta);
        return Task.CompletedTask;
    }
}

public sealed class UnitOfWorkFalso : IUnitOfWork
{
    public int GuardarCambiosInvocaciones { get; private set; }
    public int IniciarTransaccionInvocaciones { get; private set; }

    public Task<IApplicationTransaction> IniciarTransaccionAsync(CancellationToken cancellationToken = default)
    {
        IniciarTransaccionInvocaciones++;
        return Task.FromResult<IApplicationTransaction>(new TransaccionAplicacionFalsa());
    }

    public Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        GuardarCambiosInvocaciones++;
        return Task.FromResult(1);
    }

    private sealed class TransaccionAplicacionFalsa : IApplicationTransaction
    {
        public Task ConfirmarAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
