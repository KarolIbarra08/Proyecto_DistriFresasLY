using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Application.Productos;

public sealed record ActualizarProductoFresaCommand(
    int Id,
    int? Calibre,
    string? Calidad,
    double? Peso,
    string? Descripcion);

public sealed class ActualizarProductoFresaUseCase
{
    private readonly IProductoRepository _productos;

    public ActualizarProductoFresaUseCase(IProductoRepository productos) => _productos = productos;

    public async Task<Result<ProductoFresaDetalle>> EjecutarAsync(
        ActualizarProductoFresaCommand comando,
        CancellationToken cancellationToken = default)
    {
        var producto = await _productos.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (producto is not ProductoFresa fresa)
        {
            return Result.Failure<ProductoFresaDetalle>(ProductoErrores.FresaNoEncontrada(comando.Id));
        }

        int calibreFinal = comando.Calibre ?? fresa.Calibre.Value;
        string calidadFinal = string.IsNullOrWhiteSpace(comando.Calidad) ? fresa.Calidad : comando.Calidad;
        double pesoFinal = comando.Peso ?? fresa.Peso.Value;
        string descripcionFinal = string.IsNullOrWhiteSpace(comando.Descripcion) ? fresa.Descripcion : comando.Descripcion;

        bool existeCalibreOtro = await _productos.ExisteCalibreAsync(calibreFinal, comando.Id, cancellationToken);

        var updateResult = fresa.Actualizar(
            calibreFinal,
            calidadFinal,
            pesoFinal,
            descripcionFinal,
            _ => existeCalibreOtro);

        if (updateResult.IsFailure)
        {
            return Result.Failure<ProductoFresaDetalle>(updateResult.Error);
        }

        await _productos.ActualizarFresaAsync(fresa, cancellationToken);

        return Result.Success(new ProductoFresaDetalle(
            fresa.Id,
            fresa.TipoProducto.ToString(),
            fresa.Calibre.Value,
            fresa.Calidad,
            fresa.Peso.Value,
            fresa.Descripcion));
    }
}