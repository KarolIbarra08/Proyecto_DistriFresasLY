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
        CancellationToken cancellationToken)
    {
        var recepcion = await recepcionRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<RecepcionResponse>(
                Error.NotFound("Recepcion.NotFound", $"No se encontró una recepción con el Id: {id}"))
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
