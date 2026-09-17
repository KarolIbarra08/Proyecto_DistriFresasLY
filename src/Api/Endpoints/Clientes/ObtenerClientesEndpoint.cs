using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public class ObtenerClientesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/clientes", Manejador)
           .WithName("ObtenerClientes")
           .WithTags("Clientes")
           .WithSummary("Obtiene la lista de clientes registrados");
    }

    private static IResult Manejador()
    {
        var result = Result.Success(ClienteDataStore.ClientesDb.AsReadOnly());
        return result.ToHttpResult();
    }
}