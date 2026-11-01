using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ObtenerDetallesRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones/{id:int}/detalles", Manejador)
           .WithName("ObtenerDetallesRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Muestra los detalles de una recepción");
    }

    private static async Task<IResult> Manejador(
        int id,
        IRecepcionRepository recepcionRepository,
        CancellationToken cancellationToken)
    {
        var recepcion = await recepcionRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<List<DetalleRecepcionResponse>>(
                Error.NotFound("Recepcion.NotFound",
                    $"No se encontró la recepción con ID: {id}"))
                .ToHttpResult();
        }

        var detalles = recepcion.Detalles
            .Select(d => new DetalleRecepcionResponse(
                d.Id,
                d.IdRecepcion,
                d.IdProducto,
                (int)d.Cantidad,
                d.PrecioUnitario,
                d.Subtotal,
                d.FechaIngreso))
            .ToList();

        return Result.Success(detalles).ToHttpResult();
    }
}
