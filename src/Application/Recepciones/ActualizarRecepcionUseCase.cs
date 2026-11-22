using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Recepciones;

public sealed record ActualizarRecepcionCommand(
    int Id,
    int? PedidoId);

public sealed class ActualizarRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;
    private readonly IPedidoRepository _pedidos;

    public ActualizarRecepcionUseCase(
        IRecepcionRepository recepciones,
        IPedidoRepository pedidos)
    {
        _recepciones = recepciones;
        _pedidos = pedidos;
    }

    public async Task<Result<RecepcionDetalleDto>> EjecutarAsync(
        ActualizarRecepcionCommand comando,
        CancellationToken cancellationToken = default)
    {
        var recepcion = await _recepciones.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<RecepcionDetalleDto>(RecepcionErrores.NoEncontrada(comando.Id));
        }

        if (recepcion.ProcesadaEnInventario)
        {
            return Result.Failure<RecepcionDetalleDto>(RecepcionErrores.YaProcesada);
        }

        if (!comando.PedidoId.HasValue || comando.PedidoId.Value <= 0)
        {
            return Result.Failure<RecepcionDetalleDto>(RecepcionErrores.PedidoIdInvalido);
        }

        var pedidoExiste = await _pedidos.ObtenerPorIdAsync(comando.PedidoId.Value, cancellationToken);
        if (pedidoExiste is null)
        {
            return Result.Failure<RecepcionDetalleDto>(RecepcionErrores.PedidoNoEncontrado(comando.PedidoId.Value));
        }

        var actualizarResult = recepcion.Actualizar();
        if (actualizarResult.IsFailure)
        {
            return Result.Failure<RecepcionDetalleDto>(actualizarResult.Error);
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