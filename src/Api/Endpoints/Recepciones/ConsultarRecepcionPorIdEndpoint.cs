using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ConsultarRecepcionPorIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones/{id:int}", Manejador)
           .WithName("ConsultarRecepcionPorId")
           .WithTags("Recepciones")
           .WithSummary("Consulta una recepción por ID");
    }

    private static IResult Manejador(int id)
    {
        var recepcion = RecepcionDataStore.RecepcionesDb.FirstOrDefault(r => r.Id == id);
        if (recepcion is null)
        {
            return Result.Failure<RecepcionResponse>(Error.NotFound(
                "Recepcion.NotFound", $"No se encontró la recepción con ID: {id}"))
                .ToHttpResult();
        }

        return Result.Success(recepcion).ToHttpResult();
    }
}