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

        // Guardamos la cantidad anterior
        var cantidadAnterior = detalle.Cantidad;

        // Diferencia entre nueva y anterior
        var diferencia =
            request.Cantidad - cantidadAnterior;

        // Si aumenta la cantidad, aumenta inventario.
        if (diferencia > 0)
        {
            InventarioDataStore.AumentarStock(
                detalle.IdProducto,
                diferencia);
        }

        // Si disminuye la cantidad, disminuye inventario.
        if (diferencia < 0)
        {
            var resultado =
                InventarioDataStore.DisminuirStock(
                    detalle.IdProducto,
                    Math.Abs(diferencia));

            if (!resultado.exito)
            {
                return Result.Failure<DetalleRecepcionResponse>(
                    Error.Validation(
                        "Inventario.Error",
                        resultado.mensajeError ??
                        "No fue posible actualizar el inventario."))
                    .ToHttpResult();
            }
        }

        var result = detalle.Actualizar(
            request.Cantidad,
            request.UnidadMedida,
            request.TipoMedida,
            request.PrecioUnitario);

        if (result.IsFailure)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                result.Error)
                .ToHttpResult();
        }

        var response = new DetalleRecepcionResponse(
            detalle.Id,
            detalle.IdRecepcion,
            detalle.IdProducto,
            detalle.Cantidad,
            detalle.UnidadMedida,
            detalle.TipoMedida,
            detalle.PrecioUnitario,
            detalle.Subtotal);

        return Result.Success(response)
            .ToHttpResult();
    }
}