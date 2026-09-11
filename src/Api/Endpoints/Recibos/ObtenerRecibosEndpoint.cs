using DistriFresasLY.Api.Contracts.Recibos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recibos;

public class ObtenerRecibosEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recibos", Manejador)
           .WithName("ObtenerRecibos")
           .WithTags("Recibos")
           .WithSummary("Obtiene la lista completa de recibos generados");
    }

    private static IResult Manejador()
    {
        var lista = ReciboDataStore.RecibosDb.Select(r => new ReciboResponse(
            r.Id,
            r.FechaRecibo,
            r.NumeroRecibo,
            r.Total,
            r.VentaId
        )).ToList();

        return Result.Success(lista).ToHttpResult();
    }
}