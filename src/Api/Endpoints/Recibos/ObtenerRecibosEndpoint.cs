using DistriFresasLY.Api.Contracts.Recibos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recibos;

public class ObtenerRecibosEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recibos", Manejador)
           .WithName("ObtenerRecibos")
           .WithTags("Recibos")
           .WithSummary("Obtiene la lista completa de recibos generados");
    }

    private static async Task<IResult> Manejador(
        IReciboRepository reciboRepository,
        CancellationToken cancellationToken)
    {
        var lista = await reciboRepository.ObtenerTodosAsync(cancellationToken);

        var response = lista.Select(r => new ReciboResponse(
            r.Id,
            r.FechaRecibo,
            r.NumeroRecibo,
            r.Total,
            r.VentaId)).ToList();

        return Result.Success(response).ToHttpResult();
    }
}
