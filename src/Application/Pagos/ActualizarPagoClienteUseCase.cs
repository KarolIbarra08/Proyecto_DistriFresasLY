using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pagos;

public sealed record ActualizarPagoClienteCommand(
    int Id,
    int? ClienteId,
    int? VentaId,
    double? Valor,
    string? MedioPago,
    DateTime? FechaPago,
    string? Estado);

public sealed class ActualizarPagoClienteUseCase
{
    private readonly IPagoClienteRepository _pagos;

    public ActualizarPagoClienteUseCase(IPagoClienteRepository pagos) => _pagos = pagos;

    public async Task<Result<PagoClienteDetalleDto>> EjecutarAsync(
        ActualizarPagoClienteCommand comando,
        CancellationToken cancellationToken = default)
    {
        var pago = await _pagos.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (pago is null)
        {
            return Result.Failure<PagoClienteDetalleDto>(
                PagoClienteErrores.NoEncontrado(comando.Id));
        }

        var actualizarResult = pago.Actualizar(
            comando.ClienteId,
            comando.VentaId,
            comando.Valor,
            comando.MedioPago,
            comando.FechaPago,
            comando.Estado);

        if (actualizarResult.IsFailure)
        {
            return Result.Failure<PagoClienteDetalleDto>(actualizarResult.Error);
        }

        await _pagos.ActualizarAsync(pago, cancellationToken);

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
