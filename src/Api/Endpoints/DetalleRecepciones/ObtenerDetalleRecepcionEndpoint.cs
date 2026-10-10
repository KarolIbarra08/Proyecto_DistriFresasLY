using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.DetalleRecepciones;

public class ObtenerDetalleRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/detalles-recepcion/{id:int}",
            Manejador)
           .WithName("ObtenerDetalleRecepcion")
           .WithTags("DetalleRecepciones")
           .WithSummary("Obtiene un detalle de recepción por ID");
    }

    private static IResult Manejador(int id)
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