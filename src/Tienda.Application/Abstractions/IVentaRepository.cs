using Tienda.Domain.Entities;

namespace Tienda.Application.Abstractions;

/// <summary>
/// Define la persistencia de ventas requerida por la aplicación.
/// </summary>
public interface IVentaRepository
{
    /// <summary>
    /// Agrega la venta al contexto de persistencia de la operación actual.
    /// </summary>
    /// <param name="venta">Venta que se agregará con sus líneas e importes históricos.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    Task AgregarAsync(Venta venta, CancellationToken cancellationToken = default);
}
