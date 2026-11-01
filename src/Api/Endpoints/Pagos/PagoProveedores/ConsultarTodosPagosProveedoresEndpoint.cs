using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos.Proveedores;

public class ConsultarTodosPagosProveedoresEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/pagos/proveedores", Manejador)
           .WithName("ConsultarTodosPagosProveedores")
           .WithTags("Pagos - Proveedores")
           .WithSummary("Consulta todos los pagos a proveedores");
    }

    private static async Task<IResult> Manejador(
        IPagoProveedorRepository pagoRepository,
        CancellationToken cancellationToken)
    {
        var pagos = await pagoRepository.ObtenerTodosAsync(cancellationToken);

        var response = pagos.Select(pago => new PagoProveedorResponse(
            pago.Id,
            pago.ProveedorId,
            pago.PedidoId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaPago,
            pago.Estado)).ToList();

        return Result.Success(response).ToHttpResult();
    }
}
