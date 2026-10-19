using Tienda.Domain.Exceptions;

namespace Tienda.Domain.ValueObjects;

public sealed class DocumentoIdentidad : IEquatable<DocumentoIdentidad>
{
    public string Valor { get; }

    public DocumentoIdentidad(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new DomainRuleException("El documento de identidad es obligatorio.");
        }

        Valor = valor.Trim().ToUpperInvariant();
    }

    public bool Equals(DocumentoIdentidad? other)
    {
        if (other is null)
        {
            return false;
        }

        return Valor == other.Valor;
    }

    public override bool Equals(object? obj) => Equals(obj as DocumentoIdentidad);

    public override int GetHashCode() => Valor.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Valor;
}
