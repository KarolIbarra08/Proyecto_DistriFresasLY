using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ObtenerRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones/{id:int}", Manejador)
           .WithName("ObtenerRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Obtiene una recepción por su ID");
    }

    private static IResult Manejador(int id)
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

        var response = new RecepcionResponse(
            recepcion.Id,
            recepcion.FechaRecepcion,
            recepcion.ValorPago);

        return Result.Success(response)
            .ToHttpResult();
    }
}