using DistriFresasLY.Api.Endpoints.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.DetalleRecepciones;

public class EliminarDetalleRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "/detalles-recepcion/{id:int}",
            Manejador)
           .WithName("EliminarDetalleRecepcion")
           .WithTags("DetalleRecepciones")
           .WithSummary("Elimina un detalle y actualiza el inventario");
    }

    private static IResult Manejador(int id)
    {
        var detalle =
            DetalleRecepcionDataStore.DetallesDb
                .FirstOrDefault(d => d.Id == id);

        if (detalle is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "DetalleRecepcion.NotFound",
                    $"No se encontró el detalle con ID: {id}"))
                .ToHttpResult();
        }

        // Al eliminar el detalle,
        // se debe retirar del inventario la cantidad que había agregado.
        var resultado =
            InventarioDataStore.DisminuirStock(
                detalle.IdProducto,
                detalle.Cantidad);

        if (!resultado.exito)
        {
            return Result.Failure(
                Error.Validation(
                    "Inventario.Error",
                    resultado.mensajeError ??
                    "No fue posible actualizar el inventario."))
                .ToHttpResult();
        }

        DetalleRecepcionDataStore.DetallesDb
            .Remove(detalle);

        return Result.Success()
            .ToHttpResult();
    }
}