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
           .WithSummary("Actualiza la información de una venta existente");
    }

    private static IResult Manejador(int id, ActualizarVentaRequest request)
    {
        var index = VentaDataStore.VentasDb.FindIndex(v => v.Id == id);
        if (index == -1)
        {
            Result<VentaResponse> errorResult = Error.NotFound(
                "Venta.NotFound",
                $"No se encontró una venta con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var ventaExistente = VentaDataStore.VentasDb[index];

        decimal nuevoTotal = ventaExistente.Total;
        if (request.Detalles is not null && request.Detalles.Any())
        {
            nuevoTotal = request.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
        }

        var ventaActualizada = ventaExistente with
        {
            ClienteId = request.ClienteId ?? ventaExistente.ClienteId,
            Estado = !string.IsNullOrWhiteSpace(request.Estado) ? request.Estado.Trim() : ventaExistente.Estado,
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