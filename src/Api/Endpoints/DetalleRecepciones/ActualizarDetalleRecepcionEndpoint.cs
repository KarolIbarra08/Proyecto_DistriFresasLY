using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Endpoints.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.DetalleRecepciones;

public class ActualizarDetalleRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/detalles-recepcion/{id:int}",
            Manejador)
           .WithName("ActualizarDetalleRecepcion")
           .WithTags("DetalleRecepciones")
           .WithSummary("Actualiza un detalle de recepción");
    }

    private static IResult Manejador(
        int id,
        ActualizarDetalleRecepcionRequest request)
    {
        var detalle =
            DetalleRecepcionDataStore.DetallesDb
                .FirstOrDefault(d => d.Id == id);

        if (detalle is null)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.NotFound(
                    "DetalleRecepcion.NotFound",
                    $"No se encontró el detalle con ID: {id}"))
                .ToHttpResult();
        }


       var updateResult = detalle.Actualizar(
                request.Cantidad,
                request.PrecioUnitario
            );

            if (updateResult.IsFailure)
            {
                return Results.BadRequest(updateResult.Error);
            }

        var response = new DetalleRecepcionResponse(
                detalle.Id,
                detalle.IdRecepcion,
                detalle.IdProducto,
                (int)detalle.Cantidad,
                detalle.PrecioUnitario,
                detalle.Subtotal,
                detalle.FechaIngreso
            );

        return Result.Success(response)
            .ToHttpResult();
    }
}