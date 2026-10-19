using Tienda.Domain.Abstractions;
using Tienda.Domain.Exceptions;
using Tienda.Domain.ValueObjects;

namespace Tienda.Domain.Entities;

public sealed class Cliente : Entity
{
    public string NombreCompleto { get; private set; }
    public DocumentoIdentidad Documento { get; private set; }
    public bool Activo { get; private set; }

    public Cliente(Guid id, string nombreCompleto, DocumentoIdentidad documento, bool activo = true)
    {
        if (id == Guid.Empty)
        {
            throw new DomainRuleException("El identificador del cliente es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(nombreCompleto))
        {
            throw new DomainRuleException("El nombre del cliente es obligatorio.");
        }

        Documento = documento ?? throw new DomainRuleException("El documento del cliente es obligatorio.");

        Id = id;
        NombreCompleto = nombreCompleto.Trim();
        Activo = activo;
    }

    public void Desactivar()
    {
        Activo = false;
    }
}
