using Tienda.Application.Abstractions;
using Tienda.Domain.Entities;
using Tienda.Domain.Exceptions;
using Tienda.Domain.ValueObjects;

namespace Tienda.Application.Catalogos;

/// <summary>
/// Ejecuta operaciones de aplicación para el catálogo de proveedores.
/// </summary>
public sealed class ProveedorService(IProveedorRepository proveedores, IUnitOfWork unidadDeTrabajo)
{
    /// <summary>
    /// Registra un proveedor si su NIT aún no está utilizado.
    /// </summary>
    /// <param name="razonSocial">Razón social del proveedor.</param>
    /// <param name="nit">NIT del proveedor.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El proveedor creado.</returns>
    /// <exception cref="DomainRuleException">Si el NIT ya existe o los datos incumplen reglas del dominio.</exception>
    public async Task<Proveedor> CrearAsync(
        string razonSocial,
        string nit,
        CancellationToken cancellationToken = default)
    {
        var nitNormalizado = new Nit(nit);
        if (await proveedores.ExisteNitAsync(nitNormalizado.Valor, cancellationToken: cancellationToken))
        {
            throw new DomainRuleException("Ya existe un proveedor con el NIT indicado.");
        }

        var proveedor = new Proveedor(Guid.NewGuid(), razonSocial, nitNormalizado);
        await proveedores.AgregarAsync(proveedor, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        return proveedor;
    }
}
