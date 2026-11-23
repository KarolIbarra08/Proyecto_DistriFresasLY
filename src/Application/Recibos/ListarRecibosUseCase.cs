using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Recibos;

public sealed class ListarRecibosUseCase
{
    private readonly IReciboRepository _recibos;

    public ListarRecibosUseCase(IReciboRepository recibos) => _recibos = recibos;

    public async Task<Result<IReadOnlyList<ReciboDetalleDto>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var recibos = await _recibos.ObtenerTodosAsync(cancellationToken);

        var lista = recibos.Select(r => new ReciboDetalleDto(
            r.Id,
            r.FechaRecibo,
            r.NumeroRecibo,
            r.Total,
            r.VentaId)).ToList();

        return Result.Success<IReadOnlyList<ReciboDetalleDto>>(lista);
    }
}
