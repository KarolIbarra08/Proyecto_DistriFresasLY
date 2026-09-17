using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public class ObtenerVentaPorIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/ventas/{id:int}", Manejador)
           .WithName("ObtenerVentaPorId")
           .WithTags("Ventas")
           .WithSummary("Consulta una venta especifica por su ID");
    }

    private static IResult Manejador(int id)
    {
        var venta = VentaDataStore.VentasDb.FirstOrDefault(v => v.Id == id);
        if (venta is null)
        {
            return Result.Failure<VentaResponse>(Error.NotFound(
                "Venta.NotFound",
                $"No se encontro una venta con el Id: {id}"))
                .ToHttpResult();
        }

        var response = new VentaResponse(
            venta.Id,
            venta.FechaVenta,
            venta.Total,
            venta.Estado,
            venta.ClienteId
        );

        return Result.Success(response).ToHttpResult();
    }
}