using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pagos;

public sealed class ObtenerPagoProveedorUseCase
{
    private readonly IPagoProveedorRepository _pagos;

    public ObtenerPagoProveedorUseCase(IPagoProveedorRepository pagos) => _pagos = pagos;

    public async Task<Result<PagoProveedorDetalleDto>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var pago = await _pagos.ObtenerPorIdAsync(id, cancellationToken);
        if (pago is null)
        {
            return Result.Failure<PagoProveedorDetalleDto>(
                PagoProveedorErrores.NoEncontrado(id));
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
