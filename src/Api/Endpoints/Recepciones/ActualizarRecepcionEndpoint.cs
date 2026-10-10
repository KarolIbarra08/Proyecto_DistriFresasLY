using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ActualizarRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/recepciones/{id:int}", Manejador)
           .WithName("ActualizarRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Actualiza una recepción existente");
    }

    private static IResult Manejador(
        int id,
        ActualizarRecepcionRequest request)
    {
        var recepcion =
            RecepcionDataStore.ObtenerPorId(id);

        if (recepcion is null)
        {
            return Result.Failure<RecepcionResponse>(
                Error.NotFound(
                    "Recepcion.NotFound",
                    $"No se encontró la recepción con ID: {id}"))
                .ToHttpResult();
        }

        var result = recepcion.Actualizar(
            request.ValorPago,
            request.FechaRecepcion);

        if (result.IsFailure)
        {
            return Result.Failure<RecepcionResponse>(
                result.Error)
                .ToHttpResult();
        }

        var response = new RecepcionResponse(
            recepcion.Id,
            recepcion.FechaRecepcion,
            recepcion.ValorPago);

        return Result.Success(response)
            .ToHttpResult();
    }
}