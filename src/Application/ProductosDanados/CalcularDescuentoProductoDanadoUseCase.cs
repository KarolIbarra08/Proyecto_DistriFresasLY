using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.ProductosDanados;

public sealed class CalcularDescuentoProductoDanadoUseCase
{
    private readonly IProductoDanadoRepository _danados;
    private readonly IRecepcionRepository _recepciones;

    public CalcularDescuentoProductoDanadoUseCase(
        IProductoDanadoRepository danados,
        IRecepcionRepository recepciones)
    {
        _danados = danados;
        _recepciones = recepciones;
    }

    public async Task<Result<CalculoDescuentoDetalle>> EjecutarAsync(
        int id,
        int? recepcionId,
        CancellationToken cancellationToken = default)
    {
        var entidad = await _danados.ObtenerPorIdAsync(id, cancellationToken);
        if (entidad is null)
        {
            return Result.Failure<CalculoDescuentoDetalle>(ProductoDanadoErrores.NoEncontrado(id));
        }

        var recepciones = await _recepciones.ObtenerTodosAsync(cancellationToken);

        Domain.Entities.Recepciones.Recepcion? recepcionConProducto;

        if (recepcionId.HasValue && recepcionId.Value > 0)
        {
            recepcionConProducto = recepciones
                .FirstOrDefault(r => r.Id == recepcionId.Value && r.Detalles.Any(d => d.IdProducto == entidad.ProductoId));

            if (recepcionConProducto is null)
            {
                return Result.Failure<CalculoDescuentoDetalle>(
                    ProductoDanadoErrores.RecepcionNoEncontradaEspecifica(recepcionId.Value, entidad.ProductoId));
            }
        }
        else
        {
            recepcionConProducto = recepciones
                .Where(r => r.Detalles.Any(d => d.IdProducto == entidad.ProductoId))
                .OrderByDescending(r => r.Id)
                .FirstOrDefault();
        }

        if (recepcionConProducto is null)
        {
            return Result.Failure<CalculoDescuentoDetalle>(
                ProductoDanadoErrores.RecepcionNoEncontradaProducto(entidad.ProductoId));
        }

        var detalleProducto = recepcionConProducto.Detalles.First(d => d.IdProducto == entidad.ProductoId);
        decimal precioUnitarioDecimal = (decimal)detalleProducto.PrecioUnitario;

        var calculoResult = entidad.CalcularDescuento(precioUnitarioDecimal);
        if (calculoResult.IsFailure)
        {
            return Result.Failure<CalculoDescuentoDetalle>(calculoResult.Error);
        }

        await _danados.ActualizarAsync(entidad, cancellationToken);

        return Result.Success(new CalculoDescuentoDetalle(
            entidad.Id,
            entidad.ProductoId,
            recepcionConProducto.Id,
            entidad.TipoProducto,
            (int)entidad.Cantidad,
            detalleProducto.PrecioUnitario,
            entidad.FechaProductoDanado,
            entidad.Motivo,
            entidad.Descuento is not null
                ? new DescuentoCalculadoDetalle(
                    entidad.Descuento.Valor,
                    entidad.Descuento.Fecha,
                    entidad.Descuento.Motivo)
                : null));
    }
}