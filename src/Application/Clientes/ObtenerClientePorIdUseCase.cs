using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Clientes;

public sealed class ObtenerClientePorIdUseCase
{
    private readonly IClienteRepository _clientes;

    public ObtenerClientePorIdUseCase(IClienteRepository clientes)
    {
        _clientes = clientes;
    }

    public async Task<Result<ClienteDetalle>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var cliente = await _clientes.ObtenerPorIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            return ClienteErrores.NoEncontrado(id);
        }

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