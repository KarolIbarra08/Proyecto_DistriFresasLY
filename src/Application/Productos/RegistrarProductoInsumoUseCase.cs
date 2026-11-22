using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Application.Productos;

public sealed record RegistrarProductoInsumoCommand(
    string Nombre,
    string TipoInsumo,
    string UnidadMedida,
    string Descripcion);

public sealed class RegistrarProductoInsumoUseCase
{
    private readonly IProductoRepository _productos;

    public RegistrarProductoInsumoUseCase(IProductoRepository productos) => _productos = productos;

    public async Task<Result<ProductoInsumoDetalle>> EjecutarAsync(
        RegistrarProductoInsumoCommand comando,
        CancellationToken cancellationToken = default)
    {
        var result = ProductoInsumo.Create(
            comando.Nombre,
            comando.TipoInsumo,
            comando.UnidadMedida,
            comando.Descripcion);

        if (result.IsFailure)
        {
            return Result.Failure<ProductoInsumoDetalle>(result.Error);
        }

        var insumo = result.Value;
        var idGenerado = await _productos.AgregarInsumoAsync(insumo, cancellationToken);

        return Result.Success(new ProductoInsumoDetalle(
            idGenerado,
            insumo.TipoProducto.ToString(),
            insumo.Nombre,
            insumo.TipoInsumo,
            insumo.UnidadMedida,
            insumo.Descripcion));
    }
}