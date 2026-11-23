using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Ventas;

public sealed class ObtenerVentaUseCase
{
    private readonly IVentaRepository _ventas;

    public ObtenerVentaUseCase(IVentaRepository ventas) => _ventas = ventas;

    public async Task<Result<VentaDetalleDto>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var venta = await _ventas.ObtenerPorIdAsync(id, cancellationToken);
        if (venta is null)
        {
            return Result.Failure<VentaDetalleDto>(VentaErrores.NoEncontrada(id));
        }

        return Result.Success(new VentaDetalleDto(
            venta.Id,
            venta.FechaVenta,
            venta.Total,
            venta.Estado.Value,
            venta.ClienteId == 0 ? null : venta.ClienteId));
    }
}
