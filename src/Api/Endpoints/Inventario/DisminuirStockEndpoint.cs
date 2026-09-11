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
        var index = InventarioDataStore.InventarioDb.FindIndex(i => i.ProductoId == request.ProductoId);
        if (index == -1)
        {
            Result<InventarioResponse> errorResult = Error.NotFound(
                "Inventario.NotFound",
                $"No existe registro de inventario para el producto: {request.ProductoId}");

            return errorResult.ToHttpResult();
        }

        var actual = InventarioDataStore.InventarioDb[index];

        var actualizado = actual with
        {
            CantidadDisponible = actual.CantidadDisponible - request.Cantidad,
            FechaActualizacion = DateTime.Now
        };

        InventarioDataStore.InventarioDb[index] = actualizado;

        var response = new InventarioResponse(
            actualizado.Id,
            actualizado.ProductoId,
            actualizado.CantidadDisponible,
            actualizado.FechaActualizacion
        );

        return Result.Success(response).ToHttpResult();
    }
}