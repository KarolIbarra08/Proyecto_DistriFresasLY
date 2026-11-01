using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public class ObtenerVentasEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/ventas", Manejador)
           .WithName("ObtenerVentas")
           .WithTags("Ventas")
           .WithSummary("Obtiene todas las ventas registradas");
    }

    private static async Task<IResult> Manejador(
        IVentaRepository ventaRepository,
        CancellationToken cancellationToken)
    {
        var ventas = await ventaRepository.ObtenerTodosAsync(cancellationToken);

        var response = ventas.Select(v => new VentaResponse(
            v.Id,
            v.FechaVenta,
            v.Total,
            v.Estado.Value,
            v.ClienteId == 0 ? null : v.ClienteId)).ToList();

        return Result.Success(response).ToHttpResult();
    }
}
