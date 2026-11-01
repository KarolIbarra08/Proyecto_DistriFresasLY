using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ActualizarRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/recepciones/{id:int}", Manejador)
           .WithName("ActualizarRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Actualiza los datos de una recepción existente");
    }

    private static async Task<IResult> Manejador(
        int id,
        ActualizarRecepcionRequest request,
        IRecepcionRepository recepcionRepository,
        IPedidoRepository pedidoRepository,
        CancellationToken cancellationToken)
    {
        var recepcion = await recepcionRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<RecepcionResponse>(
                Error.NotFound("Recepcion.NotFound", $"No se encontró una recepción con el Id: {id}"))
                .ToHttpResult();
        }

       
        if (recepcion.ProcesadaEnInventario)
        {
            return Result.Failure<RecepcionResponse>(
                Error.Validation("Recepcion.YaProcesada", "No se puede editar una recepción cuyo inventario ya ha sido procesado."))
                .ToHttpResult();
        }

        if (!request.PedidoId.HasValue || request.PedidoId.Value <= 0)
        {
            return Result.Failure<RecepcionResponse>(
                Error.Validation("Recepcion.PedidoIdInvalido", "El PedidoId es obligatorio y debe ser mayor a 0."))
                .ToHttpResult();
        }

        int pedidoId = request.PedidoId.Value;

        var pedidoExiste = await pedidoRepository.ObtenerPorIdAsync(pedidoId, cancellationToken);
        if (pedidoExiste is null)
        {
            return Result.Failure<RecepcionResponse>(
                Error.NotFound("Pedido.NotFound", $"No existe el pedido con ID {pedidoId}."))
                .ToHttpResult();
        }


        var actualizarResult = recepcion.Actualizar();
        if (actualizarResult.IsFailure)
        {
            return Result.Failure<RecepcionResponse>(actualizarResult.Error).ToHttpResult();
        }

        await recepcionRepository.ActualizarAsync(recepcion, cancellationToken);

        var response = new RecepcionResponse(
            recepcion.Id,
            recepcion.PedidoId,
            recepcion.FechaRecepcion,
            recepcion.ValorPago,
            recepcion.ProcesadaEnInventario,
            recepcion.CalcularTotal());

        return Result.Success(response).ToHttpResult();
    }
}