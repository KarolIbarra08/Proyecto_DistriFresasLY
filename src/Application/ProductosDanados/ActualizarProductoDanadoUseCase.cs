using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.ProductosDanados;

namespace DistriFresasLY.Application.ProductosDanados;

public sealed record ActualizarProductoDanadoCommand(
    int Id,
    int? Cantidad,
    string? Identificacion,
    decimal? Valor,
    DateTime? FechaProductoDanado,
    string? Motivo);

public sealed class ActualizarProductoDanadoUseCase
{
    private readonly IProductoDanadoRepository _danados;
    private readonly IInventarioRepository _inventarios;

    public ActualizarProductoDanadoUseCase(
        IProductoDanadoRepository danados,
        IInventarioRepository inventarios)
    {
        _danados = danados;
        _inventarios = inventarios;
    }

    public async Task<Result<ProductoDanadoDetalle>> EjecutarAsync(
        ActualizarProductoDanadoCommand comando,
        CancellationToken cancellationToken = default)
    {
        var actual = await _danados.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (actual is null)
        {
            return Result.Failure<ProductoDanadoDetalle>(ProductoDanadoErrores.NoEncontrado(comando.Id));
        }

        int cantidadFinal = comando.Cantidad ?? actual.Cantidad.Value;
        if (cantidadFinal <= 0)
        {
            return Result.Failure<ProductoDanadoDetalle>(ProductoDanadoErrores.DatosInvalidos);
        }

        string identificacionFinal = string.IsNullOrWhiteSpace(comando.Identificacion) ? actual.Identificacion : comando.Identificacion;
        decimal valorFinal = comando.Valor ?? actual.Valor;
        DateTime fechaFinal = comando.FechaProductoDanado is { Year: > 1970 }
            ? comando.FechaProductoDanado.Value
            : actual.FechaProductoDanado;
        string motivoFinal = string.IsNullOrWhiteSpace(comando.Motivo) ? actual.Motivo : comando.Motivo;

        var actualizadaResult = ProductoDanado.Create(
            actual.ProductoId,
            actual.TipoProducto,
            cantidadFinal,
            identificacionFinal,
            valorFinal,
            fechaFinal,
            motivoFinal,
            id: actual.Id);

        if (actualizadaResult.IsFailure)
        {
            return Result.Failure<ProductoDanadoDetalle>(actualizadaResult.Error);
        }

        int diferencia = cantidadFinal - actual.Cantidad.Value;
        if (diferencia != 0)
        {
            var inventario = await _inventarios.ObtenerPorProductoIdAsync(actual.ProductoId, cancellationToken);
            if (inventario is null)
            {
                return Result.Failure<ProductoDanadoDetalle>(ProductoDanadoErrores.InventarioNoEncontrado(actual.ProductoId));
            }

            if (diferencia > 0)
            {
                var dec = inventario.Disminuir(diferencia);
                if (dec.IsFailure) return Result.Failure<ProductoDanadoDetalle>(dec.Error);
            }
            else
            {
                var inc = inventario.Aumentar(Math.Abs(diferencia));
                if (inc.IsFailure) return Result.Failure<ProductoDanadoDetalle>(inc.Error);
            }

            await _inventarios.ActualizarAsync(inventario, cancellationToken);
        }

        var entidad = actualizadaResult.Value;
        await _danados.ActualizarAsync(entidad, cancellationToken);

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