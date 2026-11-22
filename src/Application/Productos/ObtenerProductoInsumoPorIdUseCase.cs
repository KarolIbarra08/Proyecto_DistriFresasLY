using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Application.Productos;

public sealed class ObtenerProductoInsumoPorIdUseCase
{
    private readonly IProductoRepository _productos;

    public ObtenerProductoInsumoPorIdUseCase(IProductoRepository productos) => _productos = productos;

    public async Task<Result<ProductoInsumoDetalle>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var producto = await _productos.ObtenerPorIdAsync(id, cancellationToken);
        if (producto is not ProductoInsumo insumo)
        {
            return Result.Failure<ProductoInsumoDetalle>(ProductoErrores.InsumoNoEncontrado(id));
        }

        return Result.Success(new ProductoInsumoDetalle(
            insumo.Id,
            insumo.TipoProducto.ToString(),
            insumo.Nombre,
            insumo.TipoInsumo,
            insumo.UnidadMedida,
            insumo.Descripcion));
    }
}