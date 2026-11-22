using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Pedidos;

namespace DistriFresasLY.Application.Pedidos;

public sealed record ActualizarPedidoCommand(
    int Id,
    int? ProveedorId,
    string? Estado,
    IReadOnlyList<CrearDetallePedidoCommand>? Detalles);

public sealed class ActualizarPedidoUseCase
{
    private readonly IPedidoRepository _pedidos;
    private readonly IProveedorRepository _proveedores;
    private readonly IPagoProveedorRepository _pagos;

    public ActualizarPedidoUseCase(
        IPedidoRepository pedidos,
        IProveedorRepository proveedores,
        IPagoProveedorRepository pagos)
    {
        _pedidos = pedidos;
        _proveedores = proveedores;
        _pagos = pagos;
    }

    public async Task<Result<PedidoDetalleDto>> EjecutarAsync(
        ActualizarPedidoCommand comando,
        CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidos.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (pedido is null)
        {
            return Result.Failure<PedidoDetalleDto>(PedidoErrores.NoEncontrado(comando.Id));
        }

        if (comando.ProveedorId.HasValue)
        {
            if (comando.ProveedorId.Value <= 0)
            {
                return Result.Failure<PedidoDetalleDto>(PedidoErrores.ProveedorInvalido);
            }

            var proveedor = await _proveedores.ObtenerPorIdAsync(comando.ProveedorId.Value, cancellationToken);
            if (proveedor is null)
            {
                return Result.Failure<PedidoDetalleDto>(PedidoErrores.ProveedorNoEncontrado(comando.ProveedorId.Value));
            }

            if (comando.ProveedorId.Value != pedido.ProveedorId)
            {
                var pagos = await _pagos.ObtenerTodosAsync(cancellationToken);
                if (pagos.Any(p => p.PedidoId == comando.Id))
                {
                    return Result.Failure<PedidoDetalleDto>(PedidoErrores.ConPagosAsociadosCambiarProveedor);
                }
            }

            var proveedorResult = pedido.ActualizarProveedor(comando.ProveedorId.Value);
            if (proveedorResult.IsFailure)
            {
                return Result.Failure<PedidoDetalleDto>(proveedorResult.Error);
            }
        }

        if (!string.IsNullOrWhiteSpace(comando.Estado))
        {
            var estadoResult = pedido.ActualizarEstado(comando.Estado);
            if (estadoResult.IsFailure)
            {
                return Result.Failure<PedidoDetalleDto>(estadoResult.Error);
            }
        }

        if (comando.Detalles is not null)
        {
            var detalles = new List<DetallePedido>();

            foreach (var detReq in comando.Detalles)
            {
                var detalleResult = DetallePedido.Create(
                    pedidoId: comando.Id,
                    descripcion: detReq.Descripcion,
                    cantidad: detReq.Cantidad,
                    valorUnitario: detReq.ValorUnitario);

                if (detalleResult.IsFailure)
                {
                    return Result.Failure<PedidoDetalleDto>(detalleResult.Error);
                }

                detalles.Add(detalleResult.Value);
            }

            var detallesResult = pedido.ActualizarDetalles(detalles);
            if (detallesResult.IsFailure)
            {
                return Result.Failure<PedidoDetalleDto>(detallesResult.Error);
            }
        }

        await _pedidos.ActualizarAsync(pedido, cancellationToken);

        var actualizado = await _pedidos.ObtenerPorIdAsync(comando.Id, cancellationToken);

        return Result.Success(new PedidoDetalleDto(
            actualizado!.Id,
            actualizado.ProveedorId,
            actualizado.FechaPedido,
            actualizado.Estado,
            actualizado.Total,
            actualizado.Detalles.Select(d => new DetallePedidoItemDto(
                d.Id,
                d.PedidoId,
                d.Descripcion,
                d.Cantidad.Value,
                d.ValorUnitario,
                d.Subtotal)).ToList()));
    }
}