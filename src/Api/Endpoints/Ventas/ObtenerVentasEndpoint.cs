using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public class ObtenerVentasEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/ventas", Manejador)
           .WithName("ObtenerVentas")
           .WithTags("Ventas")
           .WithSummary("Consulta el listado general de ventas");
    }

    private static IResult Manejador()
    {
        var lista = VentaDataStore.VentasDb.Select(v => new VentaResponse(
            v.Id,
            v.FechaVenta,
            v.Total,
            v.Estado,
            v.ClienteId
        )).ToList();

        Result<List<VentaResponse>> successResult = Result.Success(lista);
        return successResult.ToHttpResult();
    }
}