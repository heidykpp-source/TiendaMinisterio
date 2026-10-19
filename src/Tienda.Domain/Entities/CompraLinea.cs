using Tienda.Domain.Exceptions;

namespace Tienda.Domain.Entities;

public sealed class CompraLinea
{
    public Guid ProductoId { get; }
    public int Cantidad { get; }
    public decimal CostoUnitario { get; }

    public CompraLinea(Guid productoId, int cantidad, decimal costoUnitario)
    {
        if (productoId == Guid.Empty)
        {
            throw new DomainRuleException("El producto de la línea de compra es obligatorio.");
        }

        if (cantidad <= 0)
        {
            throw new DomainRuleException("La cantidad de la línea de compra debe ser mayor a cero.");
        }

        if (costoUnitario <= 0)
        {
            throw new DomainRuleException("El costo unitario de la línea de compra debe ser mayor a cero.");
        }

        ProductoId = productoId;
        Cantidad = cantidad;
        CostoUnitario = costoUnitario;
    }
}
