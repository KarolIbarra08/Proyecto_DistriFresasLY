using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Application.Productos;

public sealed record ActualizarProductoInsumoCommand(
    int Id,
    string Nombre,
    string TipoInsumo,
    string UnidadMedida,
    string Descripcion);

public sealed class ActualizarProductoInsumoUseCase
{
    private readonly IProductoRepository _productos;

    public ActualizarProductoInsumoUseCase(IProductoRepository productos) => _productos = productos;

    public async Task<Result<ProductoInsumoDetalle>> EjecutarAsync(
        ActualizarProductoInsumoCommand comando,
        CancellationToken cancellationToken = default)
    {
        var producto = await _productos.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (producto is not ProductoInsumo insumo)
        {
            return Result.Failure<ProductoInsumoDetalle>(ProductoErrores.InsumoNoEncontrado(comando.Id));
        }

        var updateResult = insumo.ActualizarDatos(
            comando.Nombre,
            comando.TipoInsumo,
            comando.UnidadMedida,
            comando.Descripcion);

        if (updateResult.IsFailure)
        {
            return Result.Failure<ProductoInsumoDetalle>(updateResult.Error);
        }

        await _productos.ActualizarInsumoAsync(insumo, cancellationToken);

        return Result.Success(new ProductoInsumoDetalle(
            insumo.Id,
            insumo.TipoProducto.ToString(),
            insumo.Nombre,
            insumo.TipoInsumo,
            insumo.UnidadMedida,
            insumo.Descripcion));
    }
}