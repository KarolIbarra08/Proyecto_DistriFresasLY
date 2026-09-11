using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
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

    private static IResult Manejador(int ventaId)
    {
        var venta = VentaDataStore.VentasDb.FirstOrDefault(v => v.Id == ventaId);
        if (venta is null)
        {
            return Result.Failure<List<DetalleVentaResponse>>(Error.NotFound(
                "Venta.NotFound",
                $"No se encontro una venta con el Id: {ventaId}"))
                .ToHttpResult();
        }

        var detalles = (venta.Detalles ?? Enumerable.Empty<DetalleVentaModel>())
            .Select(d => new DetalleVentaResponse(
                d.ProductoId,
                d.Cantidad,
                d.PrecioUnitario,
                d.Subtotal,
                venta.Total
            )).ToList();

        return Result.Success(detalles).ToHttpResult();
    }
}