using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class CalcularTotalRecepcionesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones/{id:int}/total", Manejador)
           .WithName("CalcularTotalRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Calcula el valor total de una recepción");
    }

    private static async Task<IResult> Manejador(
        int id,
        IRecepcionRepository recepcionRepository,
        CancellationToken cancellationToken)
    {
        var recepcion = await recepcionRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (recepcion is null)
        {
            Result<double> errorResult = Error.NotFound(
                "Recepcion.NotFound",
                $"No se encontró una recepción con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        return Result.Success(recepcion.CalcularTotal()).ToHttpResult();
    }
}
