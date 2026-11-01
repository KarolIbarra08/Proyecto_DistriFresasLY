using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class EliminarRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/recepciones/{id:int}", Manejador)
           .WithName("EliminarRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Elimina una recepción por su ID");
    }

    private static async Task<IResult> Manejador(
        int id,
        IRecepcionRepository recepcionRepository,
        CancellationToken cancellationToken)
    {
        var recepcion = await recepcionRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure(Error.NotFound("Recepcion.NotFound", $"No se encontró la recepción con ID {id}."))
                .ToHttpResult();
        }

        await recepcionRepository.EliminarAsync(id, cancellationToken);
        return Result.Success().ToHttpResult();
    }
}
