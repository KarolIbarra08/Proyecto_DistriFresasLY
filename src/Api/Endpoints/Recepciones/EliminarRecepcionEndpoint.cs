using DistriFresasLY.Api.Endpoints.DetalleRecepciones;
using DistriFresasLY.Api.Endpoints.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class EliminarRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/recepciones/{id:int}", Manejador)
           .WithName("EliminarRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Elimina una recepción y sus detalles");
    }

    private static IResult Manejador(int id)
    {
        var recepcion =
            RecepcionDataStore.ObtenerPorId(id);

        if (recepcion is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Recepcion.NotFound",
                    $"No se encontró la recepción con ID: {id}"))
                .ToHttpResult();
        }

        var detalles = DetalleRecepcionDataStore.DetallesDb
            .Where(d => d.IdRecepcion == id)
            .ToList();


        foreach (var detalle in detalles)
        {
            var resultado = InventarioDataStore.DisminuirStock(
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
        }


        DetalleRecepcionDataStore.DetallesDb
            .RemoveAll(d => d.IdRecepcion == id);


        RecepcionDataStore.RecepcionesDb
            .Remove(recepcion);

        return Result.Success()
            .ToHttpResult();
    }
}