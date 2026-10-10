using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public record TotalRecepcionesResponse(
    double TotalPagado,
    int TotalRecepciones
);

public class CalcularTotalRecepcionesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones/total", Manejador)
           .WithName("CalcularTotalRecepciones")
           .WithTags("Recepciones")
           .WithSummary("Calcula el valor total pagado y el número de recepciones registradas");
    }

    private static IResult Manejador()
    {
        var totalPagado =
            RecepcionDataStore.RecepcionesDb.Sum(r => r.ValorPago);

        var totalRecepciones =
            RecepcionDataStore.RecepcionesDb.Count;

        var response = new TotalRecepcionesResponse(
            totalPagado,
            totalRecepciones);

        return Result.Success(response).ToHttpResult();
    }
}