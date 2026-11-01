using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public class ObtenerVentaPorIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/ventas/{id:int}", Manejador)
           .WithName("ObtenerVentaPorId")
           .WithTags("Ventas")
           .WithSummary("Obtiene una venta por su ID");
    }

    private static async Task<IResult> Manejador(
        int id,
        IVentaRepository ventaRepository,
        CancellationToken cancellationToken)
    {
        var venta = await ventaRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (venta is null)
        {
            return Result.Failure<VentaResponse>(
                Error.NotFound("Venta.NotFound", $"No se encontró una venta con el ID: {id}"))
                .ToHttpResult();
        }

        var response = new VentaResponse(
            venta.Id,
            venta.FechaVenta,
            venta.Total,
            venta.Estado.Value,
            venta.ClienteId == 0 ? null : venta.ClienteId);

        return Result.Success(response).ToHttpResult();
    }
}
