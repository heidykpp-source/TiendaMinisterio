using Tienda.Application.Abstractions;
using Tienda.Domain.Entities;
using Tienda.Domain.Exceptions;
using Tienda.Domain.ValueObjects;

namespace Tienda.Application.Catalogos;

/// <summary>
/// Ejecuta operaciones de aplicación para el catálogo de clientes.
/// </summary>
public sealed class ClienteService(IClienteRepository clientes, IUnitOfWork unidadDeTrabajo)
{
    /// <summary>
    /// Registra un cliente si su documento aún no está utilizado.
    /// </summary>
    /// <param name="nombreCompleto">Nombre completo del cliente.</param>
    /// <param name="documento">Documento de identidad.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El cliente creado.</returns>
    /// <exception cref="DomainRuleException">Si el documento ya existe o los datos incumplen reglas del dominio.</exception>
    public async Task<Cliente> CrearAsync(
        string nombreCompleto,
        string documento,
        CancellationToken cancellationToken = default)
    {
        var documentoIdentidad = new DocumentoIdentidad(documento);
        if (await clientes.ExisteDocumentoAsync(documentoIdentidad.Valor, cancellationToken: cancellationToken))
        {
            throw new DomainRuleException("Ya existe un cliente con el documento indicado.");
        }

        var cliente = new Cliente(Guid.NewGuid(), nombreCompleto, documentoIdentidad);
        await clientes.AgregarAsync(cliente, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        return cliente;
    }
}
