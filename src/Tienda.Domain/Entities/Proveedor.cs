using Tienda.Domain.Abstractions;
using Tienda.Domain.Exceptions;
using Tienda.Domain.ValueObjects;

namespace Tienda.Domain.Entities;

public sealed class Proveedor : Entity
{
    public string RazonSocial { get; private set; }
    public Nit Nit { get; private set; }
    public bool Activo { get; private set; }

    public Proveedor(Guid id, string razonSocial, Nit nit, bool activo = true)
    {
        if (id == Guid.Empty)
        {
            throw new DomainRuleException("El identificador del proveedor es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(razonSocial))
        {
            throw new DomainRuleException("La razón social del proveedor es obligatoria.");
        }

        Nit = nit ?? throw new DomainRuleException("El NIT del proveedor es obligatorio.");

        Id = id;
        RazonSocial = razonSocial.Trim();
        Activo = activo;
    }

    public void Desactivar()
    {
        Activo = false;
    }
}
