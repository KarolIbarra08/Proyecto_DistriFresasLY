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
            Result<bool> errorResult = Error.NotFound(
                "Venta.NotFound",
                $"No se encontró una venta con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        VentaDataStore.VentasDb.RemoveAt(index);

        Result<bool> successResult = Result.Success(true);
        return successResult.ToHttpResult();
    }
}