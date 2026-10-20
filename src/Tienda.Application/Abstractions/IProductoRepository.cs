using Tienda.Domain.Entities;

namespace Tienda.Application.Abstractions;

/// <summary>
/// Define las operaciones de persistencia de productos requeridas por la aplicación.
/// </summary>
public interface IProductoRepository
{
    /// <summary>
    /// Comprueba si ya existe un producto con el SKU indicado.
    /// </summary>
    /// <param name="sku">SKU normalizado del producto.</param>
    /// <param name="excluirProductoId">Identificador que se excluye de la comprobación, si aplica.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns><see langword="true"/> si el SKU ya está registrado.</returns>
    Task<bool> ExisteSkuAsync(string sku, Guid? excluirProductoId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un producto por su identificador.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El producto encontrado o <see langword="null"/>.</returns>
    Task<Producto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Comprueba si el producto tiene compras o ventas asociadas.
    /// </summary>
    /// <param name="productoId">Identificador del producto.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns><see langword="true"/> si hay movimientos asociados.</returns>
    Task<bool> TieneMovimientosAsync(Guid productoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene los productos cuyos identificadores se indican.
    /// </summary>
    /// <param name="ids">Identificadores de productos a buscar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Los productos encontrados; los identificadores inexistentes no aparecen en la colección.</returns>
    Task<IReadOnlyList<Producto>> ObtenerPorIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un producto al contexto de persistencia de la operación actual.
    /// </summary>
    /// <param name="producto">Producto que se agregará.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    Task AgregarAsync(Producto producto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca el producto para actualizarse en la operación de persistencia actual.
    /// </summary>
    /// <param name="producto">Producto que se actualizará.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    Task ActualizarAsync(Producto producto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un producto sin movimientos del contexto de persistencia actual.
    /// </summary>
    /// <param name="producto">Producto que se eliminará.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    Task EliminarAsync(Producto producto, CancellationToken cancellationToken = default);
}
