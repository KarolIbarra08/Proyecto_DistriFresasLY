using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Application.Productos;

public sealed record RegistrarProductoFresaCommand(
    int Calibre,
    string Calidad,
    double Peso,
    string Descripcion);

public sealed class RegistrarProductoFresaUseCase
{
    private readonly IProductoRepository _productos;

    public RegistrarProductoFresaUseCase(IProductoRepository productos) => _productos = productos;

    public async Task<Result<ProductoFresaDetalle>> EjecutarAsync(
        RegistrarProductoFresaCommand comando,
        CancellationToken cancellationToken = default)
    {
        var existeCalibre = await _productos.ExisteCalibreAsync(comando.Calibre, null, cancellationToken);

        var result = ProductoFresa.Create(
            comando.Calibre,
            comando.Calidad,
            comando.Peso,
            comando.Descripcion,
            _ => existeCalibre,
            id: 0);

        if (result.IsFailure)
        {
            return Result.Failure<ProductoFresaDetalle>(result.Error);
        }

        var fresa = result.Value;
        var idGenerado = await _productos.AgregarFresaAsync(fresa, cancellationToken);

        return Result.Success(new ProductoFresaDetalle(
            idGenerado,
            fresa.TipoProducto.ToString(),
            fresa.Calibre.Value,
            fresa.Calidad,
            fresa.Peso.Value,
            fresa.Descripcion));
    }
}