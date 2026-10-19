using Tienda.Domain.Exceptions;

namespace Tienda.Domain.ValueObjects;

public sealed class Nit : IEquatable<Nit>
{
    public string Valor { get; }

    public Nit(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new DomainRuleException("El NIT es obligatorio.");
        }

        Valor = valor.Trim().ToUpperInvariant();
    }

    public bool Equals(Nit? other)
    {
        if (other is null)
        {
            return false;
        }

        return Valor == other.Valor;
    }

    public override bool Equals(object? obj) => Equals(obj as Nit);

    public override int GetHashCode() => Valor.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Valor;
}
