using DistriFresasLY.Api.Extensions;
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

    private static IResult Manejador(int id)
    {
        var recepcion = RecepcionDataStore.RecepcionesDb.FirstOrDefault(r => r.Id == id);
        if (recepcion is null)
        {
            Result<double> errorResult = Error.NotFound(
                "Recepcion.NotFound",
                $"No se encontró una recepción con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        double total = recepcion.CalcularTotal();

        return Result.Success(total).ToHttpResult();
    }
}