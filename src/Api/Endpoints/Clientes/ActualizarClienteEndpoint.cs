using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public class ActualizarClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/clientes/{id:int}", Manejador)
           .WithName("ActualizarCliente")
           .WithTags("Clientes")
           .WithSummary("Actualiza los datos de un cliente existente");
    }

    private static IResult Manejador(int id, ActualizarClienteRequest request)
    {
        var index = ClienteDataStore.ClientesDb.FindIndex(cliente => cliente.Id == id);
        if (index == -1)
        {
            Result<ClienteResponse> errorResult = Error.NotFound(
                "Cliente.NotFound",
                $"No se encontro un cliente con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var clienteActualizado = new ClienteResponse(
            Id: id,
            Nombre: request.Nombre.Trim(),
            CedulaNit: request.CedulaNit.Trim(),
            Telefono: request.Telefono?.Trim() ?? string.Empty,
            Direccion: request.Direccion?.Trim() ?? string.Empty,
            TipoNegocio: request.TipoNegocio?.Trim() ?? string.Empty
        );

        ClienteDataStore.ClientesDb[index] = clienteActualizado;

        Result<ClienteResponse> successResult = Result.Success(clienteActualizado);
        return successResult.ToHttpResult();
    }
}