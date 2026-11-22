using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pedidos;

public sealed class EliminarPedidoUseCase
{
    private readonly IPedidoRepository _pedidos;
    private readonly IPagoProveedorRepository _pagos;

    public EliminarPedidoUseCase(
        IPedidoRepository pedidos,
        IPagoProveedorRepository pagos)
    {
        _pedidos = pedidos;
        _pagos = pagos;
    }

    public async Task<Result<bool>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidos.ObtenerPorIdAsync(id, cancellationToken);
        if (pedido is null)
        {
            return Result.Failure<bool>(PedidoErrores.NoEncontrado(id));
        }

        var pagos = await _pagos.ObtenerTodosAsync(cancellationToken);
        if (pagos.Any(p => p.PedidoId == id))
        {
            return Result.Failure<bool>(PedidoErrores.ConPagosAsociadosEliminar);
        }

        await _pedidos.EliminarAsync(id, cancellationToken);
        return Result.Success(true);
    }
}