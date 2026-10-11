using DistriFresasLY.Api.Contracts.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Inventario;

public class AjustarStockEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/inventario/ajustar", Manejador)
           .WithName("AjustarStock")
           .WithTags("Inventario");
    }

    private static IResult Manejador(AjustarStockRequest request)
    {
        // Ajusta o reinicia el stock base del producto
        InventarioDataStore.InicializarStock(request.ProductoId, request.Cantidad);
        
        var inventario = InventarioDataStore.ObtenerPorProductoId(request.ProductoId);
        
        if (inventario is null)
        {
            return Result.Failure(Error.NotFound("Inventario.NotFound", "No se encontró el registro de inventario."))
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

public record AjustarStockRequest(int ProductoId, int Cantidad);