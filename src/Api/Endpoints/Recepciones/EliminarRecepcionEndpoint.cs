using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class EliminarRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/recepciones/{id:int}", Manejador)
           .WithName("EliminarRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Elimina una recepción por ID");
    }

    private static IResult Manejador(int id)
    {
        var index = RecepcionDataStore.RecepcionesDb.FindIndex(r => r.Id == id);
        if (index == -1)
        {
            return Result.Failure<bool>(Error.NotFound(
                "Recepcion.NotFound", $"No se encontró la recepción con ID: {id}"))
                .ToHttpResult();
        }

        RecepcionDataStore.RecepcionesDb.RemoveAt(index);

        return Result.Success(true).ToHttpResult();
    }
}