using Tienda.Domain.Entities;

namespace Tienda.Application.Abstractions;

/// <summary>
/// Define las operaciones de persistencia de proveedores requeridas por la aplicación.
/// </summary>
public interface IProveedorRepository
{
    /// <summary>
    /// Comprueba si ya existe un proveedor con el NIT indicado.
    /// </summary>
    /// <param name="nit">NIT normalizado del proveedor.</param>
    /// <param name="excluirProveedorId">Identificador que se excluye de la comprobación, si aplica.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns><see langword="true"/> si el NIT ya está registrado.</returns>
    Task<bool> ExisteNitAsync(string nit, Guid? excluirProveedorId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un proveedor al contexto de persistencia de la operación actual.
    /// </summary>
    /// <param name="proveedor">Proveedor que se agregará.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    Task AgregarAsync(Proveedor proveedor, CancellationToken cancellationToken = default);
}
