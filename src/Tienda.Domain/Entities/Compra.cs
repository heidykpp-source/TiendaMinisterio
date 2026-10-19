using Tienda.Domain.Abstractions;
using Tienda.Domain.Exceptions;

namespace Tienda.Domain.Entities;

public sealed class Compra : Entity
{
    private readonly List<CompraLinea> _lineas;

    public Guid ProveedorId { get; }
    public DateTime Fecha { get; }
    public IReadOnlyCollection<CompraLinea> Lineas => _lineas.AsReadOnly();

    public Compra(Guid id, Guid proveedorId, IEnumerable<CompraLinea> lineas, DateTime? fecha = null)
    {
        if (id == Guid.Empty)
        {
            throw new DomainRuleException("El identificador de la compra es obligatorio.");
        }

        if (proveedorId == Guid.Empty)
        {
            throw new DomainRuleException("El proveedor de la compra es obligatorio.");
        }

        if (lineas is null)
        {
            throw new DomainRuleException("La compra debe tener al menos una línea.");
        }

        _lineas = lineas.ToList();

        if (_lineas.Count == 0)
        {
            throw new DomainRuleException("La compra debe tener al menos una línea.");
        }

        Id = id;
        ProveedorId = proveedorId;
        Fecha = fecha ?? DateTime.UtcNow;
    }
}
