using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        IRecepcionRepository recepcionRepository,
        CancellationToken cancellationToken)
    {
        var recepciones = await recepcionRepository.ObtenerTodosAsync(cancellationToken);

        var response = recepciones
            .SelectMany(r => r.Detalles)
            .Select(d => new DetalleRecepcionResponse(
                d.Id,
                d.IdRecepcion,
                d.IdProducto,
                (int)d.Cantidad,
                d.PrecioUnitario,
                d.Subtotal,
                d.FechaIngreso))
            .ToList();

        return Result.Success(response).ToHttpResult();
    }
}
