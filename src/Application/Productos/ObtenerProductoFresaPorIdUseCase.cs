using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Application.Productos;

public sealed class ObtenerProductoFresaPorIdUseCase
{
    private readonly IProductoRepository _productos;

    public ObtenerProductoFresaPorIdUseCase(IProductoRepository productos) => _productos = productos;

    public async Task<Result<ProductoFresaDetalle>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var producto = await _productos.ObtenerPorIdAsync(id, cancellationToken);
        if (producto is not ProductoFresa fresa)
        {
            return Result.Failure<ProductoFresaDetalle>(ProductoErrores.FresaNoEncontrada(id));
        }

        return Result.Success(new ProductoFresaDetalle(
            fresa.Id,
            fresa.TipoProducto.ToString(),
            fresa.Calibre.Value,
            fresa.Calidad,
            fresa.Peso.Value,
            fresa.Descripcion));
    }
}