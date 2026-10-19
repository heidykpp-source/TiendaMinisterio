using Tienda.Domain.Abstractions;
using Tienda.Domain.Exceptions;

namespace Tienda.Domain.Entities;

public sealed class Venta : Entity
{
    private readonly List<VentaLinea> _lineas;

    public Guid ClienteId { get; }
    public DateTime Fecha { get; }
    public IReadOnlyCollection<VentaLinea> Lineas => _lineas.AsReadOnly();

    public Venta(Guid id, Guid clienteId, IEnumerable<VentaLinea> lineas, DateTime? fecha = null)
    {
        if (id == Guid.Empty)
        {
            throw new DomainRuleException("El identificador de la venta es obligatorio.");
        }

        if (clienteId == Guid.Empty)
        {
            throw new DomainRuleException("El cliente de la venta es obligatorio.");
        }

        if (lineas is null)
        {
            throw new DomainRuleException("La venta debe tener al menos una línea.");
        }

        _lineas = lineas.ToList();

        if (_lineas.Count == 0)
        {
            throw new DomainRuleException("La venta debe tener al menos una línea.");
        }

        Id = id;
        ClienteId = clienteId;
        Fecha = fecha ?? DateTime.UtcNow;
    }
}
