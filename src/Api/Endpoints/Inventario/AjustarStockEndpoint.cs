using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Inventario;

public class AjustarStockEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/inventario/ajustar", Manejador)
           .WithName("AjustarStock")
           .WithTags("Inventario")
           .WithSummary("Establece un nuevo valor absoluto para el stock de inventario");
    }

    private static IResult Manejador([FromBody] ActualizarStockRequest request)
    {
        var inventario = InventarioDataStore.AjustarStock(request.ProductoId, request.Cantidad);
        if (inventario is null)
        {
            return Result.Failure<InventarioResponse>(Error.NotFound(
                "Inventario.NotFound",
                $"No existe registro de inventario para el producto: {request.ProductoId}"))
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