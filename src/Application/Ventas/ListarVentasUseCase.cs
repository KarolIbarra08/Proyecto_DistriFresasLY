using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Ventas;

public sealed class ListarVentasUseCase
{
    private readonly IVentaRepository _ventas;

    public ListarVentasUseCase(IVentaRepository ventas) => _ventas = ventas;

    public async Task<Result<IReadOnlyList<VentaDetalleDto>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var ventas = await _ventas.ObtenerTodosAsync(cancellationToken);

        var lista = ventas.Select(v => new VentaDetalleDto(
            v.Id,
            v.FechaVenta,
            v.Total,
            v.Estado.Value,
            v.ClienteId == 0 ? null : v.ClienteId)).ToList();

        return Result.Success<IReadOnlyList<VentaDetalleDto>>(lista);
    }
}
