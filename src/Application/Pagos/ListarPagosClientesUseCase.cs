using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pagos;

public sealed class ListarPagosClientesUseCase
{
    private readonly IPagoClienteRepository _pagos;

    public ListarPagosClientesUseCase(IPagoClienteRepository pagos) => _pagos = pagos;

    public async Task<Result<IReadOnlyList<PagoClienteDetalleDto>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var pagos = await _pagos.ObtenerTodosAsync(cancellationToken);

        var lista = pagos.Select(p => new PagoClienteDetalleDto(
            p.Id,
            p.ClienteId,
            p.VentaId,
            p.Valor,
            p.MedioPago,
            p.FechaPago,
            p.Estado)).ToList();

        return Result.Success<IReadOnlyList<PagoClienteDetalleDto>>(lista);
    }
}
