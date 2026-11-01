using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ProcesarRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/recepciones/{id:int}/procesar", Manejador)
           .WithName("ProcesarRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Marca una recepción como procesada e incrementa el inventario");
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
                Error.NotFound("Recepcion.NotFound", $"No se encontró una recepción con el Id: {id}"))
                .ToHttpResult();
        }

        var procesarResult = recepcion.MarcarComoProcesada();
        if (procesarResult.IsFailure)
        {
            return Result.Failure<RecepcionResponse>(procesarResult.Error).ToHttpResult();
        }

        await recepcionRepository.ActualizarAsync(recepcion, cancellationToken);

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
