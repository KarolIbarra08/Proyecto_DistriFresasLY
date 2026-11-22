using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Clientes;

public sealed record ActualizarClienteCommand(
    int Id,
    string? Nombre,
    string? CedulaNit,
    string? Telefono,
    string? Direccion,
    string? TipoNegocio);

public sealed class ActualizarClienteUseCase
{
    private readonly IClienteRepository _clientes;

    public ActualizarClienteUseCase(IClienteRepository clientes)
    {
        _clientes = clientes;
    }

    public async Task<Result<ClienteDetalle>> EjecutarAsync(
        ActualizarClienteCommand comando,
        CancellationToken cancellationToken = default)
    {
        var cliente = await _clientes.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (cliente is null)
        {
            return ClienteErrores.NoEncontrado(comando.Id);
        }

        if (!string.IsNullOrWhiteSpace(comando.CedulaNit) && cliente.Documento.Value != comando.CedulaNit)
        {
            var existeDocumento = await _clientes.ExistePorDocumentoAsync(
                comando.CedulaNit,
                comando.Id,
                cancellationToken);

            if (existeDocumento)
            {
                return ClienteErrores.CedulaNitDuplicada;
            }

            var docResult = cliente.ActualizarDocumento(comando.CedulaNit);
            if (docResult.IsFailure)
            {
                return docResult.Error;
            }
        }

        var updateResult = cliente.ActualizarDatos(
            comando.Nombre,
            comando.Telefono,
            comando.Direccion,
            comando.TipoNegocio);

        if (updateResult.IsFailure)
        {
            return updateResult.Error;
        }

        await _clientes.ActualizarAsync(cliente, cancellationToken);

        return new ClienteDetalle(
            cliente.Id,
            cliente.Nombre,
            cliente.Documento.Value,
            cliente.Telefono.Valor,
            cliente.Direccion,
            cliente.TipoNegocio,
            cliente.Activo);
    }
}