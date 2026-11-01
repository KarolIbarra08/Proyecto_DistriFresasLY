using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        [FromBody] ActualizarStockRequest request,
        IInventarioRepository inventarioRepository,
        CancellationToken cancellationToken)
    {
        var inventario = await inventarioRepository.ObtenerPorProductoIdAsync(request.ProductoId, cancellationToken);

        if (inventario is null)
        {
            return Result.Failure<InventarioResponse>(
                Error.NotFound("Inventario.NotFound", $"No se encontró inventario para el producto con ID {request.ProductoId}."))
                .ToHttpResult();
        }

        var disminuirResult = inventario.Disminuir(request.Cantidad);
        if (disminuirResult.IsFailure)
        {
            return Result.Failure<InventarioResponse>(disminuirResult.Error).ToHttpResult();
        }

        await inventarioRepository.ActualizarAsync(inventario, cancellationToken);

        var response = new InventarioResponse(
            inventario.Id,
            inventario.ProductoId,
            inventario.CantidadDisponible,
            inventario.FechaActualizacion
        );

        return Result.Success(response).ToHttpResult();
    }
}
