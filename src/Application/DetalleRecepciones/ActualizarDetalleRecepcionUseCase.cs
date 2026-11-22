using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Application.Recepciones;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.DetalleRecepciones;

public sealed record ActualizarDetalleRecepcionCommand(
    int Id,
    int Cantidad,
    double PrecioUnitario);

public sealed class ActualizarDetalleRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;

    public ActualizarDetalleRecepcionUseCase(IRecepcionRepository recepciones) => _recepciones = recepciones;

    public async Task<Result<DetalleRecepcionItemDto>> EjecutarAsync(
        ActualizarDetalleRecepcionCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (comando.Cantidad <= 0)
        {
            return Result.Failure<DetalleRecepcionItemDto>(DetalleRecepcionErrores.CantidadInvalida);
        }

        if (comando.PrecioUnitario <= 0)
        {
            return Result.Failure<DetalleRecepcionItemDto>(DetalleRecepcionErrores.PrecioInvalido);
        }

        var recepciones = await _recepciones.ObtenerTodosAsync(cancellationToken);

        var recepcion = recepciones.FirstOrDefault(r => r.Detalles.Any(d => d.Id == comando.Id));
        if (recepcion is null)
        {
            return Result.Failure<DetalleRecepcionItemDto>(DetalleRecepcionErrores.NoEncontrado(comando.Id));
        }

        if (recepcion.ProcesadaEnInventario)
        {
            return Result.Failure<DetalleRecepcionItemDto>(DetalleRecepcionErrores.RecepcionProcesada);
        }

        var detalle = recepcion.Detalles.First(d => d.Id == comando.Id);
        var idProducto = detalle.IdProducto;

        var updateResult = detalle.Actualizar(comando.Cantidad, comando.PrecioUnitario);
        if (updateResult.IsFailure)
        {
            return Result.Failure<DetalleRecepcionItemDto>(updateResult.Error);
        }

        await _recepciones.ActualizarAsync(recepcion, cancellationToken);

        var guardada = await _recepciones.ObtenerPorIdAsync(recepcion.Id, cancellationToken);
        var actualizado = guardada!.Detalles.LastOrDefault(d => d.IdProducto == idProducto);

        if (actualizado is null)
        {
            return Result.Failure<DetalleRecepcionItemDto>(DetalleRecepcionErrores.ErrorActualizacion);
        }

        return Result.Success(new DetalleRecepcionItemDto(
            actualizado.Id,
            actualizado.IdRecepcion,
            actualizado.IdProducto,
            (int)actualizado.Cantidad,
            actualizado.PrecioUnitario,
            actualizado.Subtotal,
            actualizado.FechaIngreso));
    }
}