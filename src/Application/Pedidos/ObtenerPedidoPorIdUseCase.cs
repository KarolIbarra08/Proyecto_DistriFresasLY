using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pedidos;

public sealed class ObtenerPedidoPorIdUseCase
{
    private readonly IPedidoRepository _pedidos;

    public ObtenerPedidoPorIdUseCase(IPedidoRepository pedidos)
    {
        _pedidos = pedidos;
    }

    public async Task<Result<PedidoDetalleDto>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidos.ObtenerPorIdAsync(id, cancellationToken);
        if (pedido is null)
        {
            return Result.Failure<PedidoDetalleDto>(PedidoErrores.NoEncontrado(id));
        }

        return Result.Success(new PedidoDetalleDto(
            pedido.Id,
            pedido.ProveedorId,
            pedido.FechaPedido,
            pedido.Estado,
            pedido.Total,
            pedido.Detalles.Select(d => new DetallePedidoItemDto(
                d.Id,
                d.PedidoId,
                d.Descripcion,
                d.Cantidad.Value,
                d.ValorUnitario,
                d.Subtotal)).ToList()));
    }
}