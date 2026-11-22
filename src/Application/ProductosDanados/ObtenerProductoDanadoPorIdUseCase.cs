using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.ProductosDanados;

public sealed class ObtenerProductoDanadoPorIdUseCase
{
    private readonly IProductoDanadoRepository _danados;

    public ObtenerProductoDanadoPorIdUseCase(IProductoDanadoRepository danados) => _danados = danados;

    public async Task<Result<ProductoDanadoDetalle>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entidad = await _danados.ObtenerPorIdAsync(id, cancellationToken);
        if (entidad is null)
        {
            return Result.Failure<ProductoDanadoDetalle>(ProductoDanadoErrores.NoEncontrado(id));
        }

        return Result.Success(new ProductoDanadoDetalle(
            entidad.Id,
            entidad.ProductoId,
            entidad.TipoProducto,
            entidad.Cantidad,
            entidad.Identificacion,
            (double)entidad.Valor,
            entidad.FechaProductoDanado,
            entidad.Motivo));
    }
}