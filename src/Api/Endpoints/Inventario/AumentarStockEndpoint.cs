using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Inventario;

public class AumentarStockEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/inventario/aumentar", Manejador)
           .WithName("AumentarStock")
           .WithTags("Inventario")
           .WithSummary("Registra un incremento en el stock del inventario");
    }

    private static async Task<IResult> Manejador(
        [FromBody] ActualizarStockRequest request,
        IInventarioRepository inventarioRepository,
        CancellationToken cancellationToken)
    {
        var inventario = await inventarioRepository.ObtenerPorProductoIdAsync(request.ProductoId, cancellationToken);
        if (inventario is null)
        {
            return Result.Failure<InventarioResponse>(Error.NotFound(
                "Inventario.NotFound",
                $"No existe registro de inventario para el producto: {request.ProductoId}"))
                .ToHttpResult();
        }

        var aumentarResult = inventario.Aumentar(request.Cantidad);
        if (aumentarResult.IsFailure)
        {
            return Result.Failure<InventarioResponse>(aumentarResult.Error).ToHttpResult();
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
