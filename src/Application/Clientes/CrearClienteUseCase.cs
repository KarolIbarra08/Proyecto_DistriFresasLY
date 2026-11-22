using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Cliente;

namespace DistriFresasLY.Application.Clientes;

public sealed record CrearClienteCommand(
    string Nombre,
    string CedulaNit,
    string Telefono,
    string Direccion,
    string TipoNegocio);

public sealed class CrearClienteUseCase
{
    private readonly IClienteRepository _clientes;

    public CrearClienteUseCase(IClienteRepository clientes)
    {
        _clientes = clientes;
    }

    public async Task<Result<ClienteDetalle>> EjecutarAsync(
        CrearClienteCommand comando,
        CancellationToken cancellationToken = default)
    {
        var existeDocumento = await _clientes.ExistePorDocumentoAsync(
            comando.CedulaNit,
            cancellationToken: cancellationToken);

        if (existeDocumento)
        {
            return ClienteErrores.CedulaNitDuplicada;
        }

        var clienteResult = Cliente.Create(
            comando.Nombre,
            comando.CedulaNit,
            comando.Telefono,
            comando.Direccion,
            comando.TipoNegocio);

        if (clienteResult.IsFailure)
        {
            return clienteResult.Error;
        }

        var idGenerado = await _clientes.AgregarAsync(clienteResult.Value, cancellationToken);
        var clienteCreado = await _clientes.ObtenerPorIdAsync(idGenerado, cancellationToken);

        return new ClienteDetalle(
            clienteCreado!.Id,
            clienteCreado.Nombre,
            clienteCreado.Documento.Value,
            clienteCreado.Telefono.Valor,
            clienteCreado.Direccion,
            clienteCreado.TipoNegocio,
            clienteCreado.Activo);
    }
}