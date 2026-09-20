using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Extensions;
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

    private static IResult Manejador()
    {
        Result<List<PagoProveedorResponse>> successResult = Result.Success(PagoDataStore.PagosProveedoresDb);
        return successResult.ToHttpResult();
    }
}