using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ListarRecepcionesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones", Manejador)
           .WithName("ListarRecepciones")
           .WithTags("Recepciones")
           .WithSummary("Lista todas las recepciones registradas");
    }

    private static async Task<IResult> Manejador(
        IRecepcionRepository recepcionRepository,
        CancellationToken cancellationToken)
    {
        var recepciones = await recepcionRepository.ObtenerTodosAsync(cancellationToken);

        var response = recepciones.Select(r => new RecepcionResponse(
            r.Id,
            r.PedidoId,
            r.FechaRecepcion,
            r.ValorPago,
            r.ProcesadaEnInventario,
            r.CalcularTotal())).ToList();

        return Result.Success(response).ToHttpResult();
    }
}
