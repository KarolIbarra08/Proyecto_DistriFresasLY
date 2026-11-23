using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Application.Pagos;

public sealed record RegistrarPagoProveedorCommand(
    int ProveedorId,
    int PedidoId,
    double Valor,
    string MedioPago,
    DateTime? FechaPago);

public sealed class RegistrarPagoProveedorUseCase
{
    private static readonly string[] EstadosProhibidosPedido =
    {
        "Cancelado",
        "Anulado"
    };

    private readonly IPagoProveedorRepository _pagos;
    private readonly IPedidoRepository _pedidos;

    public RegistrarPagoProveedorUseCase(
        IPagoProveedorRepository pagos,
        IPedidoRepository pedidos)
    {
        _pagos = pagos;
        _pedidos = pedidos;
    }

    public async Task<Result<PagoProveedorDetalleDto>> EjecutarAsync(
        RegistrarPagoProveedorCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (comando.Valor <= 0)
        {
            return Result.Failure<PagoProveedorDetalleDto>(PagoProveedorErrores.ValorInvalido);
        }

        var pedido = await _pedidos.ObtenerPorIdAsync(comando.PedidoId, cancellationToken);
        if (pedido is null)
        {
            return Result.Failure<PagoProveedorDetalleDto>(
                PagoProveedorErrores.PedidoNoEncontrado(comando.PedidoId));
        }

        if (pedido.ProveedorId != comando.ProveedorId)
        {
            return Result.Failure<PagoProveedorDetalleDto>(PagoProveedorErrores.ProveedorNoCoincide);
        }

        if (EstadosProhibidosPedido.Contains(pedido.Estado))
        {
            return Result.Failure<PagoProveedorDetalleDto>(
                PagoProveedorErrores.PedidoCerrado(comando.PedidoId, pedido.Estado));
        }

        var pagos = await _pagos.ObtenerTodosAsync(cancellationToken);

        var pagadoAnteriormente = pagos
            .Where(p =>
                p.PedidoId == comando.PedidoId &&
                p.Estado != "Anulado")
            .Sum(p => p.Valor);

        var saldoPendiente = pedido.Total - pagadoAnteriormente;

        if (saldoPendiente <= 0)
        {
            return Result.Failure<PagoProveedorDetalleDto>(PagoProveedorErrores.PedidoSaldado);
        }

        if (comando.Valor > saldoPendiente)
        {
            return Result.Failure<PagoProveedorDetalleDto>(
                PagoProveedorErrores.MontoExcedido(comando.Valor, saldoPendiente));
        }

        var nuevoTotalPagado = pagadoAnteriormente + comando.Valor;

        var estadoFinal = nuevoTotalPagado >= pedido.Total
            ? "Completado"
            : "Abono Parcial";

        var pagoResult = PagoProveedor.Create(
            proveedorId: comando.ProveedorId,
            pedidoId: comando.PedidoId,
            valor: comando.Valor,
            medioPago: comando.MedioPago,
            fechaPago: comando.FechaPago,
            estado: estadoFinal);

        if (pagoResult.IsFailure)
        {
            return Result.Failure<PagoProveedorDetalleDto>(pagoResult.Error);
        }

        var nuevoId = await _pagos.AgregarAsync(pagoResult.Value, cancellationToken);

        var pago = await _pagos.ObtenerPorIdAsync(nuevoId, cancellationToken);
        if (pago is null)
        {
            return Result.Failure<PagoProveedorDetalleDto>(PagoProveedorErrores.ErrorCreacion);
        }

        return Result.Success(new PagoProveedorDetalleDto(
            pago.Id,
            pago.ProveedorId,
            pago.PedidoId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaPago,
            pago.Estado));
    }
}
