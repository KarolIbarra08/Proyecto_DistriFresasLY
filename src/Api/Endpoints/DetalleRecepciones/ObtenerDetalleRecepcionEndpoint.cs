using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.DetalleRecepciones;

public class ObtenerDetalleRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/detalles-recepcion/{id:int}", Manejador)
           .WithName("ObtenerDetalleRecepcion")
           .WithTags("DetalleRecepciones")
           .WithSummary("Obtiene un detalle de recepción por ID");
    }

    private static async Task<IResult> Manejador(
        int id,
        IRecepcionRepository recepcionRepository,
        CancellationToken cancellationToken)
    {
        var recepciones = await recepcionRepository.ObtenerTodosAsync(cancellationToken);

        var detalle = recepciones
            .SelectMany(r => r.Detalles)
            .FirstOrDefault(d => d.Id == id);

        if (detalle is null)
        {
            return Result.Failure<DetalleRecepcionResponse>(Error.NotFound(
                "DetalleRecepcion.NotFound",
                $"No se encontró el detalle con ID: {id}"))
                .ToHttpResult();
        }

        var response = new DetalleRecepcionResponse(
            detalle.Id,
            detalle.IdRecepcion,
            detalle.IdProducto,
            (int)detalle.Cantidad,
            detalle.PrecioUnitario,
            detalle.Subtotal,
            detalle.FechaIngreso);

        return Result.Success(response).ToHttpResult();
    }
}
