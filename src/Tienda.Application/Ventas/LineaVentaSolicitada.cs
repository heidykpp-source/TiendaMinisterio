namespace Tienda.Application.Ventas;

/// <summary>
/// Representa una línea solicitada para una venta; no recibe el precio del cliente.
/// </summary>
/// <param name="ProductoId">Identificador del producto solicitado.</param>
/// <param name="Cantidad">Cantidad solicitada.</param>
public sealed record LineaVentaSolicitada(Guid ProductoId, int Cantidad);
