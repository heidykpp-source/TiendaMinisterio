using Tienda.Domain.Exceptions;

namespace Tienda.Domain.Entities;

public sealed class VentaLinea
{
    public Guid ProductoId { get; }
    public int Cantidad { get; }
    public decimal PrecioUnitario { get; }

    public VentaLinea(Guid productoId, int cantidad, decimal precioUnitario)
    {
        if (productoId == Guid.Empty)
        {
            throw new DomainRuleException("El producto de la línea de venta es obligatorio.");
        }

        if (cantidad <= 0)
        {
            throw new DomainRuleException("La cantidad de la línea de venta debe ser mayor a cero.");
        }

        if (precioUnitario <= 0)
        {
            throw new DomainRuleException("El precio unitario de la línea de venta debe ser mayor a cero.");
        }

        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }
}
