using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ObtenerRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones/{id:int}", Manejador)
           .WithName("ObtenerRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Obtiene una recepción por su ID");
    }

    private static async Task<IResult> Manejador(
        int id,
        IRecepcionRepository recepcionRepository,
        CancellationToken cancellationToken)
    {
        var recepcion = await recepcionRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<RecepcionResponse>(
                Error.NotFound("Recepcion.NotFound", $"No se encontró la recepción con ID {id}."))
                .ToHttpResult();
        }

        var response = new RecepcionResponse(
            recepcion.Id,
            recepcion.PedidoId,
            recepcion.FechaRecepcion,
            recepcion.ValorPago,
            recepcion.ProcesadaEnInventario,
            recepcion.CalcularTotal());

        return Result.Success(response).ToHttpResult();
    }
}
