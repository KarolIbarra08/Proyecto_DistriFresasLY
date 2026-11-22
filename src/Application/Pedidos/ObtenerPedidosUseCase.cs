using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pedidos;

public sealed class ObtenerPedidosUseCase
{
    private readonly IPedidoRepository _pedidos;

    public ObtenerPedidosUseCase(IPedidoRepository pedidos)
    {
        _pedidos = pedidos;
    }

    public async Task<Result<IReadOnlyList<PedidoDetalleDto>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var pedidos = await _pedidos.ObtenerTodosAsync(cancellationToken);

        var detalles = pedidos.Select(pedido => new PedidoDetalleDto(
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
                d.Subtotal)).ToList())).ToList();

        return Result.Success<IReadOnlyList<PedidoDetalleDto>>(detalles);
    }
}