using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public class ObtenerDetallesVentaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/ventas/{ventaId:int}/detalles", Manejador)
           .WithName("ObtenerDetallesVenta")
           .WithTags("Ventas")
           .WithSummary("Obtiene los detalles de productos de una venta junto con el total general");
    }

    private static async Task<IResult> Manejador(
        int ventaId,
        IVentaRepository ventaRepository,
        CancellationToken cancellationToken)
    {
        var venta = await ventaRepository.ObtenerPorIdAsync(ventaId, cancellationToken);
        if (venta is null)
        {
            return Result.Failure<List<DetalleVentaResponse>>(Error.NotFound(
                "Venta.NotFound",
                $"No se encontro una venta con el Id: {ventaId}"))
                .ToHttpResult();
        }

        var detalles = venta.Detalles
            .Select(d => new DetalleVentaResponse(
                d.ProductoId,
                d.Cantidad.Value,
                d.PrecioUnitario,
                d.Subtotal,
                venta.Total))
            .ToList();

        return Result.Success(detalles).ToHttpResult();
    }
}
