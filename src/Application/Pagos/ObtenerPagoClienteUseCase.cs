using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pagos;

public sealed class ObtenerPagoClienteUseCase
{
    private readonly IPagoClienteRepository _pagos;

    public ObtenerPagoClienteUseCase(IPagoClienteRepository pagos) => _pagos = pagos;

    public async Task<Result<PagoClienteDetalleDto>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var pago = await _pagos.ObtenerPorIdAsync(id, cancellationToken);
        if (pago is null)
        {
            return Result.Failure<PagoClienteDetalleDto>(PagoClienteErrores.NoEncontrado(id));
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
