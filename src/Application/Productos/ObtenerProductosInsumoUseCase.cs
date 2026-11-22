using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Productos;

public sealed class ObtenerProductosInsumoUseCase
{
    private readonly IProductoRepository _productos;

    public ObtenerProductosInsumoUseCase(IProductoRepository productos) => _productos = productos;

    public async Task<Result<IReadOnlyList<ProductoInsumoDetalle>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var insumos = await _productos.ObtenerTodosInsumosAsync(cancellationToken);

        var lista = insumos.Select(i => new ProductoInsumoDetalle(
            i.Id,
            i.TipoProducto.ToString(),
            i.Nombre,
            i.TipoInsumo,
            i.UnidadMedida,
            i.Descripcion)).ToList();

        return Result.Success<IReadOnlyList<ProductoInsumoDetalle>>(lista);
    }
}