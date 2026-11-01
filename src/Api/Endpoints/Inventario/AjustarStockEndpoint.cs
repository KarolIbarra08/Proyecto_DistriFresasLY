using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Inventario;

public class AjustarStockEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/inventario/ajustar", Manejador)
           .WithName("AjustarStock")
           .WithTags("Inventario")
           .WithSummary("Ajusta directamente el stock de un producto en el inventario");
    }

    private static async Task<IResult> Manejador(
        [FromBody] ActualizarStockRequest request,
        IInventarioRepository inventarioRepository,
        CancellationToken cancellationToken)
    {
        var inventario = await inventarioRepository.ObtenerPorProductoIdAsync(request.ProductoId, cancellationToken);
        if (inventario is null)
        {
            var nuevoInventarioResult = Domain.Entities.Inventario.Inventario.Create(request.ProductoId, request.Cantidad);
            if (nuevoInventarioResult.IsFailure)
            {
                return Result.Failure<InventarioResponse>(nuevoInventarioResult.Error).ToHttpResult();
            }

            var nuevoId = await inventarioRepository.AgregarAsync(nuevoInventarioResult.Value, cancellationToken);
            var creado = await inventarioRepository.ObtenerPorIdAsync(nuevoId, cancellationToken);

            var respCreado = new InventarioResponse(creado!.Id, creado.ProductoId, creado.CantidadDisponible, creado.FechaActualizacion);
            return Result.Success(respCreado).ToHttpResult();
        }

        var ajustarResult = inventario.Ajustar(request.Cantidad);
        if (ajustarResult.IsFailure)
        {
            return Result.Failure<InventarioResponse>(ajustarResult.Error).ToHttpResult();
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
