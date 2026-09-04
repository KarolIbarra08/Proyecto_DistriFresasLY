using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public class EliminarClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/clientes/{id:int}", Manejador)
           .WithName("EliminarCliente")
           .WithTags("Clientes")
           .WithSummary("Elimina un cliente por su ID");
    }

    private static IResult Manejador(int id)
    {
        var cliente = ClienteDataStore.ClientesDb.FirstOrDefault(cliente => cliente.Id == id);
        if (cliente is null)
        {
            Result<bool> errorResult = Error.NotFound(
                "Cliente.NotFound",
                $"No se encontro un cliente con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        ClienteDataStore.ClientesDb.Remove(cliente);

        Result<bool> successResult = Result.Success(true);
        return successResult.ToHttpResult();
    }
}