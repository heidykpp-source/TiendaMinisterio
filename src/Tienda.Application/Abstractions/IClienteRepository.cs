using Tienda.Domain.Entities;

namespace Tienda.Application.Abstractions;

/// <summary>
/// Define las operaciones de persistencia de clientes requeridas por la aplicación.
/// </summary>
public interface IClienteRepository
{
    /// <summary>
    /// Comprueba si ya existe un cliente con el documento indicado.
    /// </summary>
    /// <param name="documento">Documento normalizado del cliente.</param>
    /// <param name="excluirClienteId">Identificador que se excluye de la comprobación, si aplica.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns><see langword="true"/> si el documento ya está registrado.</returns>
    Task<bool> ExisteDocumentoAsync(string documento, Guid? excluirClienteId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un cliente por su identificador.
    /// </summary>
    /// <param name="id">Identificador del cliente.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El cliente encontrado o <see langword="null"/>.</returns>
    Task<Cliente?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un cliente al contexto de persistencia de la operación actual.
    /// </summary>
    /// <param name="cliente">Cliente que se agregará.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    Task AgregarAsync(Cliente cliente, CancellationToken cancellationToken = default);
}
