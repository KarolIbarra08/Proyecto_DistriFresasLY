using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Recepciones;

public sealed class ProcesarRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;

    public ProcesarRecepcionUseCase(IRecepcionRepository recepciones) => _recepciones = recepciones;

    public async Task<Result<RecepcionDetalleDto>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var recepcion = await _recepciones.ObtenerPorIdAsync(id, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<RecepcionDetalleDto>(RecepcionErrores.NoEncontrada(id));
        }

        var procesarResult = recepcion.MarcarComoProcesada();
        if (procesarResult.IsFailure)
        {
            return Result.Failure<RecepcionDetalleDto>(procesarResult.Error);
        }

        await _recepciones.ActualizarAsync(recepcion, cancellationToken);

        return Result.Success(new RecepcionDetalleDto(
            recepcion.Id,
            recepcion.PedidoId,
            recepcion.FechaRecepcion,
            recepcion.ValorPago,
            recepcion.ProcesadaEnInventario,
            recepcion.CalcularTotal()));
    }
}