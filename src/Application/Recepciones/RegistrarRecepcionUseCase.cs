using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Recepciones;

namespace DistriFresasLY.Application.Recepciones;

public sealed record RegistrarRecepcionCommand(
    int PedidoId,
    DateTime? FechaRecepcion);

public sealed class RegistrarRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;
    private readonly IPedidoRepository _pedidos;

    public RegistrarRecepcionUseCase(
        IRecepcionRepository recepciones,
        IPedidoRepository pedidos)
    {
        _recepciones = recepciones;
        _pedidos = pedidos;
    }

    public async Task<Result<RecepcionDetalleDto>> EjecutarAsync(
        RegistrarRecepcionCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (comando.PedidoId <= 0)
        {
            return Result.Failure<RecepcionDetalleDto>(RecepcionErrores.PedidoIdInvalido);
        }

        var pedidoExiste = await _pedidos.ObtenerPorIdAsync(comando.PedidoId, cancellationToken);
        if (pedidoExiste is null)
        {
            return Result.Failure<RecepcionDetalleDto>(RecepcionErrores.PedidoNoEncontrado(comando.PedidoId));
        }

        var fechaReal = comando.FechaRecepcion is { Year: > 1970 }
            ? comando.FechaRecepcion.Value
            : ColombiaTime.Ahora;

        var recepcionResult = Recepcion.Create(
            id: 0,
            fechaRecepcion: fechaReal,
            pedidoId: comando.PedidoId);

        if (recepcionResult.IsFailure)
        {
            return Result.Failure<RecepcionDetalleDto>(recepcionResult.Error);
        }

        var nuevoId = await _recepciones.AgregarAsync(recepcionResult.Value, cancellationToken);
        var creada = await _recepciones.ObtenerPorIdAsync(nuevoId, cancellationToken);

        if (creada is null)
        {
            return Result.Failure<RecepcionDetalleDto>(RecepcionErrores.ErrorCreacion);
        }

        return Result.Success(new RecepcionDetalleDto(
            creada.Id,
            creada.PedidoId,
            creada.FechaRecepcion,
            creada.ValorPago,
            creada.ProcesadaEnInventario,
            creada.CalcularTotal()));
    }
}