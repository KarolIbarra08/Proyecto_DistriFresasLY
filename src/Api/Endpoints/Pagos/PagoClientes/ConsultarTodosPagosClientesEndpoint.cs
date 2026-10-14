using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
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

    private static IResult Manejador()
    {
        var pagosResponse = PagoDataStore.PagosClientesDb
            .Select(pago => new PagoClienteResponse(
                pago.Id,
                pago.ClienteId,
                pago.VentaId,
                pago.Valor,
                pago.MedioPago,
                pago.FechaPago,
                pago.Estado))
            .ToList();

        return Result.Success(pagosResponse).ToHttpResult();
    }
}