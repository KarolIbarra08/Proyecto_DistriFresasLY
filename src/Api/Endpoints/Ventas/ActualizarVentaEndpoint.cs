using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Ventas;

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

        var venta = VentaDataStore.VentasDb[index];

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            var estadoResult = venta.ActualizarEstado(request.Estado.Trim());
            if (estadoResult.IsFailure)
            {
                return Result.Failure<VentaResponse>(estadoResult.Error).ToHttpResult();
            }
        }

        if (request.Detalles is not null)
        {
            var detallesResult = request.Detalles
                .Select(d => DetalleVenta.Create(d.ProductoId, d.Cantidad, d.PrecioUnitario))
                .ToList();

            var detalleInvalido = detallesResult.FirstOrDefault(r => r.IsFailure);
            if (detalleInvalido is not null)
            {
                return Result.Failure<VentaResponse>(detalleInvalido.Error).ToHttpResult();
            }

            var detalles = detallesResult.Select(r => r.Value).ToList();

            var detallesResultado = venta.ActualizarDetalles(detalles);
            if (detallesResultado.IsFailure)
            {
                return Result.Failure<VentaResponse>(detallesResultado.Error).ToHttpResult();
            }
        }

        venta.AsignarCliente(request.ClienteId ?? venta.ClienteId);

        var response = new VentaResponse(
            venta.Id,
            venta.FechaVenta,
            venta.Total,
            venta.Estado,
            venta.ClienteId
        );

        return Result.Success(response).ToHttpResult();
    }
}