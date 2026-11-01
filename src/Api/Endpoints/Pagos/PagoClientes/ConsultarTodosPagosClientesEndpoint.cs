using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos.PagoClientes;

public class ConsultarTodosPagosClientesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/pagos/clientes", Manejador)
           .WithName("ConsultarTodosPagosClientes")
           .WithTags("Pagos - Clientes")
           .WithSummary("Consulta la lista de todos los pagos de clientes");
    }

    private static async Task<IResult> Manejador(
        IPagoClienteRepository pagoRepository,
        CancellationToken cancellationToken)
    {
        var pagos = await pagoRepository.ObtenerTodosAsync(cancellationToken);

        var response = pagos.Select(pago => new PagoClienteResponse(
            pago.Id,
            pago.ClienteId,
            pago.VentaId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaPago,
            pago.Estado)).ToList();

        return Result.Success(response).ToHttpResult();
    }
}
