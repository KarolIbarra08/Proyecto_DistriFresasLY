using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Application.Pagos;

public sealed record RegistrarPagoClienteCommand(
    int ClienteId,
    int VentaId,
    double Valor,
    string MedioPago,
    DateTime? FechaPago);

public sealed class RegistrarPagoClienteUseCase
{
    private static readonly string[] EstadosProhibidosVenta =
    {
        "Cancelada",
        "Anulada"
    };

    private readonly IPagoClienteRepository _pagos;
    private readonly IVentaRepository _ventas;

    public RegistrarPagoClienteUseCase(
        IPagoClienteRepository pagos,
        IVentaRepository ventas)
    {
        _pagos = pagos;
        _ventas = ventas;
    }

    public async Task<Result<PagoClienteDetalleDto>> EjecutarAsync(
        RegistrarPagoClienteCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (comando.ClienteId <= 0 || comando.VentaId <= 0)
        {
            return Result.Failure<PagoClienteDetalleDto>(PagoClienteErrores.IdsInvalidos);
        }

        if (comando.Valor <= 0)
        {
            return Result.Failure<PagoClienteDetalleDto>(PagoClienteErrores.ValorInvalido);
        }

        var venta = await _ventas.ObtenerPorIdAsync(comando.VentaId, cancellationToken);
        if (venta is null)
        {
            return Result.Failure<PagoClienteDetalleDto>(
                PagoClienteErrores.VentaNoEncontrada(comando.VentaId));
        }

        if (venta.ClienteId != comando.ClienteId)
        {
            return Result.Failure<PagoClienteDetalleDto>(PagoClienteErrores.ClienteNoCoincide);
        }

        if (EstadosProhibidosVenta.Contains(venta.Estado.Value))
        {
            return Result.Failure<PagoClienteDetalleDto>(
                PagoClienteErrores.VentaCerrada(comando.VentaId, venta.Estado.Value));
        }

        var pagos = await _pagos.ObtenerTodosAsync(cancellationToken);

        var pagadoAnteriormente = pagos
            .Where(p =>
                p.VentaId == comando.VentaId &&
                p.Estado != "Anulado" &&
                p.Estado != "Cancelado")
            .Sum(p => p.Valor);

        var saldoPendiente = venta.Total - pagadoAnteriormente;

        if (saldoPendiente <= 0)
        {
            return Result.Failure<PagoClienteDetalleDto>(PagoClienteErrores.VentaSaldada);
        }

        if (comando.Valor > saldoPendiente)
        {
            return Result.Failure<PagoClienteDetalleDto>(
                PagoClienteErrores.MontoExcedido(comando.Valor, saldoPendiente));
        }

        var nuevoTotalPagado = pagadoAnteriormente + comando.Valor;

        var estadoFinal = nuevoTotalPagado >= venta.Total
            ? "Completado"
            : "Abono Parcial";

        var pagoResult = PagoCliente.Create(
            clienteId: comando.ClienteId,
            ventaId: comando.VentaId,
            valor: comando.Valor,
            medioPago: comando.MedioPago,
            fechaPago: comando.FechaPago,
            estado: estadoFinal);

        if (pagoResult.IsFailure)
        {
            return Result.Failure<PagoClienteDetalleDto>(pagoResult.Error);
        }

        var nuevoId = await _pagos.AgregarAsync(pagoResult.Value, cancellationToken);

        if (nuevoTotalPagado >= venta.Total)
        {
            var actualizarVentaResult = venta.ActualizarEstado("Completada");
            if (actualizarVentaResult.IsFailure)
            {
                return Result.Failure<PagoClienteDetalleDto>(actualizarVentaResult.Error);
            }

            await _ventas.ActualizarAsync(venta, cancellationToken);
        }

        var pago = await _pagos.ObtenerPorIdAsync(nuevoId, cancellationToken);
        if (pago is null)
        {
            return Result.Failure<PagoClienteDetalleDto>(PagoClienteErrores.ErrorCreacion);
        }

        return Result.Success(new PagoClienteDetalleDto(
            pago.Id,
            pago.ClienteId,
            pago.VentaId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaPago,
            pago.Estado));
    }
}
