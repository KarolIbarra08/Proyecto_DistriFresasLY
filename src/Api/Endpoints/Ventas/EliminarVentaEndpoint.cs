using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public class EliminarVentaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/ventas/{id:int}", Manejador)
           .WithName("EliminarVenta")
           .WithTags("Ventas")
           .WithSummary("Elimina un registro de venta por su ID");
    }

    private static IResult Manejador(int id)
    {
        var index = VentaDataStore.VentasDb.FindIndex(v => v.Id == id);
        if (index == -1)
        {
            return Result.Failure<bool>(Error.NotFound(
                "Venta.NotFound",
                $"No se encontró una venta con el Id: {id}"))
                .ToHttpResult();
        }

        VentaDataStore.VentasDb.RemoveAt(index);

        return Result.Success(true).ToHttpResult();
    }
}