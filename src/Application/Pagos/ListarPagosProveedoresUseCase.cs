using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pagos;

public sealed class ListarPagosProveedoresUseCase
{
    private readonly IPagoProveedorRepository _pagos;

    public ListarPagosProveedoresUseCase(IPagoProveedorRepository pagos) => _pagos = pagos;

    public async Task<Result<IReadOnlyList<PagoProveedorDetalleDto>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var pagos = await _pagos.ObtenerTodosAsync(cancellationToken);

        var lista = pagos.Select(p => new PagoProveedorDetalleDto(
            p.Id,
            p.ProveedorId,
            p.PedidoId,
            p.Valor,
            p.MedioPago,
            p.FechaPago,
            p.Estado)).ToList();

        return Result.Success<IReadOnlyList<PagoProveedorDetalleDto>>(lista);
    }
}
