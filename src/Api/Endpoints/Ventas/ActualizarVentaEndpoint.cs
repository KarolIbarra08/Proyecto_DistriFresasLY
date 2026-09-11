using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public class ActualizarVentaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/ventas/{id:int}", Manejador)
           .WithName("ActualizarVenta")
           .WithTags("Ventas")
           .WithSummary("Actualiza la informacion de una venta existente");
    }

    private static IResult Manejador(int id, ActualizarVentaRequest request)
    {
        var index = VentaDataStore.VentasDb.FindIndex(v => v.Id == id);
        if (index == -1)
        {
            return Result.Failure<VentaResponse>(Error.NotFound(
                "Venta.NotFound",
                $"No se encontro una venta con el Id: {id}"))
                .ToHttpResult();
        }

        var ventaExistente = VentaDataStore.VentasDb[index];

        List<DetalleVentaModel>? nuevosDetalles = request.Detalles?.Select(d => new DetalleVentaModel(
            d.ProductoId,
            d.Cantidad,
            d.PrecioUnitario,
            d.PrecioUnitario * d.Cantidad
        )).ToList();

        var detallesFinales = nuevosDetalles ?? ventaExistente.Detalles;
        

        decimal nuevoTotal = nuevosDetalles != null 
            ? detallesFinales.Sum(x => x.Subtotal) 
            : ventaExistente.Total;

        var ventaActualizada = ventaExistente with
        {
            ClienteId = request.ClienteId ?? ventaExistente.ClienteId,
            Estado = !string.IsNullOrWhiteSpace(request.Estado) ? request.Estado.Trim() : ventaExistente.Estado,
            Detalles = detallesFinales,
            Total = nuevoTotal
        };

        VentaDataStore.VentasDb[index] = ventaActualizada;

        var response = new VentaResponse(
            ventaActualizada.Id,
            ventaActualizada.FechaVenta,
            ventaActualizada.Total,
            ventaActualizada.Estado,
            ventaActualizada.ClienteId
        );

        return Result.Success(response).ToHttpResult();
    }
}