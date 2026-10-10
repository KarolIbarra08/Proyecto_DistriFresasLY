using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.DetalleRecepciones;

public class ListarDetallesRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/detalles-recepcion", Manejador)
           .WithName("ListarDetallesRecepcion")
           .WithTags("DetalleRecepciones")
           .WithSummary("Muestra todos los detalles de recepción");
    }

    private static IResult Manejador()
    {
        var response = DetalleRecepcionDataStore.DetallesDb
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

        return Result.Success(response)
            .ToHttpResult();
    }
}