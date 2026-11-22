using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Clientes;

public sealed class EliminarClienteUseCase
{
    private readonly IClienteRepository _clientes;

    public EliminarClienteUseCase(IClienteRepository clientes)
    {
        _clientes = clientes;
    }

    public async Task<Result> EjecutarAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clientes.ObtenerPorIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            return Result.Failure(ClienteErrores.NoEncontrado(id));
        }

        var tienePedidos = await _clientes.TienePedidosAsociadosAsync(id, cancellationToken);
        if (tienePedidos)
        {
            return Result.Failure(ClienteErrores.TienePedidosAsociados(id));
        }

        await _clientes.EliminarAsync(id, cancellationToken);
        return Result.Success();
    }
}