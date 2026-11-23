using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pagos;

public sealed record ActualizarPagoProveedorCommand(
    int Id,
    int? ProveedorId,
    int? PedidoId,
    double? Valor,
    string? MedioPago,
    DateTime? FechaPago,
    string? Estado);

public sealed class ActualizarPagoProveedorUseCase
{
    private readonly IPagoProveedorRepository _pagos;

    public ActualizarPagoProveedorUseCase(IPagoProveedorRepository pagos) => _pagos = pagos;

    public async Task<Result<PagoProveedorDetalleDto>> EjecutarAsync(
        ActualizarPagoProveedorCommand comando,
        CancellationToken cancellationToken = default)
    {
        var pago = await _pagos.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (pago is null)
        {
            return Result.Failure<PagoProveedorDetalleDto>(
                PagoProveedorErrores.NoEncontrado(comando.Id));
        }

        var actualizarResult = pago.Actualizar(
            comando.ProveedorId,
            comando.PedidoId,
            comando.Valor,
            comando.MedioPago,
            comando.FechaPago,
            comando.Estado);

        if (actualizarResult.IsFailure)
        {
            return Result.Failure<PagoProveedorDetalleDto>(actualizarResult.Error);
        }

        await _pagos.ActualizarAsync(pago, cancellationToken);

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
