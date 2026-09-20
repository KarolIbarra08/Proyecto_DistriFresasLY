using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos.Clientes;

public class ConsultarTodosPagosClientesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/pagos/clientes", Manejador)
           .WithName("ConsultarTodosPagosClientes")
           .WithTags("Pagos - Clientes")
           .WithSummary("Consulta todos los pagos de clientes");
    }

    private static IResult Manejador()
    {
        Result<List<PagoClienteResponse>> successResult = Result.Success(PagoDataStore.PagosClientesDb);
        return successResult.ToHttpResult();
    }
}