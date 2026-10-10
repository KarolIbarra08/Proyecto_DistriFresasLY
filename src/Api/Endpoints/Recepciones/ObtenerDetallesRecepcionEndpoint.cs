using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Endpoints.DetalleRecepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ObtenerDetallesRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/recepciones/{id:int}/detalles",
            Manejador)
           .WithName("ObtenerDetallesRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Muestra los detalles de una recepción");
    }

    private static IResult Manejador(int id)
    {
        var recepcion =
            RecepcionDataStore.ObtenerPorId(id);

        if (recepcion is null)
        {
            return Result.Failure<List<DetalleRecepcionResponse>>(
                Error.NotFound(
                    "Recepcion.NotFound",
                    $"No se encontró la recepción con ID: {id}"))
                .ToHttpResult();
        }

        var detalles = DetalleRecepcionDataStore.DetallesDb
            .Where(d => d.IdRecepcion == id)
            .Select(d => new DetalleRecepcionResponse(
                d.Id,
                d.IdRecepcion,
                d.IdProducto,
                d.Cantidad,
                d.UnidadMedida,
                d.TipoMedida,
                d.PrecioUnitario,
                d.Subtotal))
            .ToList();

        return Result.Success(detalles)
            .ToHttpResult();
    }
}