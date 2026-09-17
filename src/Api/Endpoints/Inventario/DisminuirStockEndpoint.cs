using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Inventario;

public class DisminuirStockEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/inventario/disminuir", Manejador)
           .WithName("DisminuirStock")
           .WithTags("Inventario")
           .WithSummary("Registra una salida de stock del inventario");
    }

    private static IResult Manejador([FromBody] ActualizarStockRequest request)
    {
        var (exito, inventario, mensajeError) = InventarioDataStore.DisminuirStock(request.ProductoId, request.Cantidad);

        if (!exito)
        {
            var tipoError = inventario is null 
                ? Error.NotFound("Inventario.NotFound", mensajeError!)
                : Error.Validation("Inventario.StockInsuficiente", mensajeError!);

            return Result.Failure<InventarioResponse>(tipoError).ToHttpResult();
        }

        var response = new InventarioResponse(
            inventario!.Id,
            inventario.ProductoId,
            inventario.CantidadDisponible,
            inventario.FechaActualizacion
        );

        return Result.Success(response).ToHttpResult();
    }
}