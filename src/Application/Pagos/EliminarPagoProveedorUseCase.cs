using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pagos;

public sealed class EliminarPagoProveedorUseCase
{
    private readonly IPagoProveedorRepository _pagos;

    public EliminarPagoProveedorUseCase(IPagoProveedorRepository pagos) => _pagos = pagos;

    public async Task<Result> EjecutarAsync(int id, CancellationToken cancellationToken = default)
    {
        var pago = await _pagos.ObtenerPorIdAsync(id, cancellationToken);
        if (pago is null)
        {
            return Result.Failure(PagoProveedorErrores.NoEncontrado(id));
        }

        await _pagos.EliminarAsync(id, cancellationToken);
        return Result.Success();
    }
}
