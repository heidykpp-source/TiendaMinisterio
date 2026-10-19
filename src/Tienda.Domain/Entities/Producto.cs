using Tienda.Domain.Abstractions;
using Tienda.Domain.Exceptions;

namespace Tienda.Domain.Entities;

public sealed class Producto : Entity
{
    public string Sku { get; private set; }
    public string Nombre { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public int Stock { get; private set; }
    public bool Activo { get; private set; }

    public Producto(Guid id, string sku, string nombre, decimal precioUnitario) : this(id, sku, nombre, precioUnitario, 0, true)
    {
    }

    public Producto(Guid id, string sku, string nombre, decimal precioUnitario, int stock, bool activo)
    {
        if (id == Guid.Empty)
        {
            throw new DomainRuleException("El identificador del producto es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new DomainRuleException("El SKU del producto es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new DomainRuleException("El nombre del producto es obligatorio.");
        }

        if (precioUnitario <= 0)
        {
            throw new DomainRuleException("El precio unitario debe ser mayor a cero.");
        }

        if (stock < 0)
        {
            throw new DomainRuleException("El stock no puede ser negativo.");
        }

        Id = id;
        Sku = sku.Trim().ToUpperInvariant();
        Nombre = nombre.Trim();
        PrecioUnitario = precioUnitario;
        Stock = stock;
        Activo = activo;
    }

    public void ActualizarPrecio(decimal nuevoPrecio)
    {
        if (nuevoPrecio <= 0)
        {
            throw new DomainRuleException("El precio unitario debe ser mayor a cero.");
        }

        PrecioUnitario = nuevoPrecio;
    }

    public void RegistrarCompra(int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new DomainRuleException("La cantidad de compra debe ser mayor a cero.");
        }

        Stock += cantidad;
    }

    public void RegistrarVenta(int cantidad)
    {
        if (!Activo)
        {
            throw new DomainRuleException("No se puede vender un producto inactivo.");
        }

        if (cantidad <= 0)
        {
            throw new DomainRuleException("La cantidad de venta debe ser mayor a cero.");
        }

        if (cantidad > Stock)
        {
            throw new DomainRuleException("No hay stock suficiente para realizar la venta.");
        }

        Stock -= cantidad;
    }

    public void Desactivar()
    {
        Activo = false;
    }
}
