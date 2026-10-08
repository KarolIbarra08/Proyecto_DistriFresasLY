using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public record TotalRecepcionesResponse(double TotalPagado, int TotalCantidad);

public class CalcularTotalRecepcionesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones/total", Manejador)
           .WithName("CalcularTotalRecepciones")
           .WithTags("Recepciones")
           .WithSummary("Calcula el acumulado total de valor pagado y cantidad de recepciones");
    }

    private static IResult Manejador()
    {
        var totalValor = RecepcionDataStore.RecepcionesDb.Sum(r => r.ValorPago);
        var totalCantidad = RecepcionDataStore.RecepcionesDb.Sum(r => r.Cantidad);

        var response = new TotalRecepcionesResponse(totalValor, totalCantidad);

        return Result.Success(response).ToHttpResult();
    }
}