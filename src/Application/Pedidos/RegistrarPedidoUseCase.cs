using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Pedidos;

namespace DistriFresasLY.Application.Pedidos;

public sealed record CrearDetallePedidoCommand(
    string Descripcion,
    int Cantidad,
    double ValorUnitario);

public sealed record RegistrarPedidoCommand(
    int ProveedorId,
    DateTime? FechaPedido,
    string? Estado,
    IReadOnlyList<CrearDetallePedidoCommand>? Detalles);

public sealed class RegistrarPedidoUseCase
{
    private readonly IPedidoRepository _pedidos;
    private readonly IProveedorRepository _proveedores;

    public RegistrarPedidoUseCase(
        IPedidoRepository pedidos,
        IProveedorRepository proveedores)
    {
        _pedidos = pedidos;
        _proveedores = proveedores;
    }

    public async Task<Result<PedidoDetalleDto>> EjecutarAsync(
        RegistrarPedidoCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (comando.ProveedorId <= 0)
        {
            return Result.Failure<PedidoDetalleDto>(PedidoErrores.ProveedorInvalido);
        }

        var proveedor = await _proveedores.ObtenerPorIdAsync(comando.ProveedorId, cancellationToken);
        if (proveedor is null)
        {
            return Result.Failure<PedidoDetalleDto>(PedidoErrores.ProveedorNoEncontrado(comando.ProveedorId));
        }

        if (comando.Detalles is null || comando.Detalles.Count == 0)
        {
            return Result.Failure<PedidoDetalleDto>(PedidoErrores.SinDetalles);
        }

        var detalles = new List<DetallePedido>();

        foreach (var detReq in comando.Detalles)
        {
            var detalleResult = DetallePedido.Create(
                pedidoId: 0,
                descripcion: detReq.Descripcion,
                cantidad: detReq.Cantidad,
                valorUnitario: detReq.ValorUnitario);

            if (detalleResult.IsFailure)
            {
                return Result.Failure<PedidoDetalleDto>(detalleResult.Error);
            }

            detalles.Add(detalleResult.Value);
        }

        var estado = string.IsNullOrWhiteSpace(comando.Estado) ? "Pendiente" : comando.Estado.Trim();
        var fechaPedido = comando.FechaPedido is { Year: > 1970 }
            ? comando.FechaPedido.Value
            : ColombiaTime.Ahora;

        var pedidoResult = Pedido.Create(
            proveedorId: comando.ProveedorId,
            detalles: detalles,
            fechaPedido: fechaPedido,
            estado: estado);

        if (pedidoResult.IsFailure)
        {
            return Result.Failure<PedidoDetalleDto>(pedidoResult.Error);
        }

        var nuevoId = await _pedidos.AgregarAsync(pedidoResult.Value, cancellationToken);
        var pedido = await _pedidos.ObtenerPorIdAsync(nuevoId, cancellationToken);

        if (pedido is null)
        {
            return Result.Failure<PedidoDetalleDto>(PedidoErrores.ErrorCreacion);
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