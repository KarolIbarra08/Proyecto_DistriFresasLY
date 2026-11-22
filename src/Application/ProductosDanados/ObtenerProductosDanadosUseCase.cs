using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.ProductosDanados;

public sealed class ObtenerProductosDanadosUseCase
{
    private readonly IProductoDanadoRepository _danados;

    public ObtenerProductosDanadosUseCase(IProductoDanadoRepository danados) => _danados = danados;

    public async Task<Result<IReadOnlyList<ProductoDanadoDetalle>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var lista = await _danados.ObtenerTodosAsync(cancellationToken);

        var detalles = lista.Select(d => new ProductoDanadoDetalle(
            d.Id,
            d.ProductoId,
            d.TipoProducto,
            d.Cantidad,
            d.Identificacion,
            (double)d.Valor,
            d.FechaProductoDanado,
            d.Motivo)).ToList();

        return Result.Success<IReadOnlyList<ProductoDanadoDetalle>>(detalles);
    }
}