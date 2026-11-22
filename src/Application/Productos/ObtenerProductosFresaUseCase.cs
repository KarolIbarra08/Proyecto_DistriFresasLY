using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Productos;

public sealed class ObtenerProductosFresaUseCase
{
    private readonly IProductoRepository _productos;

    public ObtenerProductosFresaUseCase(IProductoRepository productos) => _productos = productos;

    public async Task<Result<IReadOnlyList<ProductoFresaDetalle>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var fresas = await _productos.ObtenerTodasFresasAsync(cancellationToken);

        var lista = fresas.Select(f => new ProductoFresaDetalle(
            f.Id,
            f.TipoProducto.ToString(),
            f.Calibre.Value,
            f.Calidad,
            f.Peso.Value,
            f.Descripcion)).ToList();

        return Result.Success<IReadOnlyList<ProductoFresaDetalle>>(lista);
    }
}