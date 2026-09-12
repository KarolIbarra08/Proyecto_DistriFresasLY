using DistriFresasLY.Api.Contracts.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Inventario;

public class ConsultarStockEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/inventario/{productoId:int}", Manejador)
           .WithName("ConsultarStock")
           .WithTags("Inventario")
           .WithSummary("Consulta el stock disponible de un producto");
    }

    private static IResult Manejador(int productoId)
    {
        var inventario = InventarioDataStore.ObtenerPorProductoId(productoId);
        if (inventario is null)
        {
            return Result.Failure<InventarioResponse>(Error.NotFound(
                "Inventario.NotFound",
                $"No se encontro registro de inventario para el producto ID: {productoId}"))
                .ToHttpResult();
        }

        var response = new InventarioResponse(
            inventario.Id,
            inventario.ProductoId,
            inventario.CantidadDisponible,
            inventario.FechaActualizacion
        );

        return Result.Success(response).ToHttpResult();
    }
}