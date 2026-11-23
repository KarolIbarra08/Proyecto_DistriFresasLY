using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Ventas;

public sealed class ObtenerDetallesVentaUseCase
{
    private readonly IVentaRepository _ventas;

    public ObtenerDetallesVentaUseCase(IVentaRepository ventas) => _ventas = ventas;

    public async Task<Result<IReadOnlyList<DetalleVentaDto>>> EjecutarAsync(
        int ventaId,
        CancellationToken cancellationToken = default)
    {
        var venta = await _ventas.ObtenerPorIdAsync(ventaId, cancellationToken);
        if (venta is null)
        {
            return Result.Failure<IReadOnlyList<DetalleVentaDto>>(
                VentaErrores.NoEncontrada(ventaId));
        }

        var detalles = venta.Detalles
            .Select(d => new DetalleVentaDto(
                d.ProductoId,
                d.Cantidad.Value,
                d.PrecioUnitario,
                d.Subtotal,
                venta.Total))
            .ToList();

        return Result.Success<IReadOnlyList<DetalleVentaDto>>(detalles);
    }
}
