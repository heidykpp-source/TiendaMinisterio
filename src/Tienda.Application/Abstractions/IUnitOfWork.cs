namespace Tienda.Application.Abstractions;

/// <summary>
/// Coordina los cambios y transacciones de una operación de aplicación.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Inicia una transacción; disponerla sin confirmar debe revertir sus cambios.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>La transacción que delimita la operación.</returns>
    Task<IApplicationTransaction> IniciarTransaccionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste en una sola operación los cambios pendientes de los repositorios.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>La cantidad de elementos persistidos.</returns>
    Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Representa una transacción confirmable de aplicación.
/// </summary>
public interface IApplicationTransaction : IAsyncDisposable
{
    /// <summary>
    /// Confirma de forma atómica los cambios de la transacción.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    Task ConfirmarAsync(CancellationToken cancellationToken = default);
}
