using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public class ActualizarClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/clientes/{id:int}", Manejador)
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
                $"No se encontró un cliente con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var clienteExistente = ClienteDataStore.ClientesDb[index];

      
        var clienteActualizado = clienteExistente with
        {
            Nombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre.Trim() : clienteExistente.Nombre,
            CedulaNit = !string.IsNullOrWhiteSpace(request.CedulaNit) ? request.CedulaNit.Trim() : clienteExistente.CedulaNit,
            Telefono = !string.IsNullOrWhiteSpace(request.Telefono) ? request.Telefono.Trim() : clienteExistente.Telefono,
            Direccion = !string.IsNullOrWhiteSpace(request.Direccion) ? request.Direccion.Trim() : clienteExistente.Direccion,
            TipoNegocio = !string.IsNullOrWhiteSpace(request.TipoNegocio) ? request.TipoNegocio.Trim() : clienteExistente.TipoNegocio
        };

        ClienteDataStore.ClientesDb[index] = clienteActualizado;

        Result<ClienteResponse> successResult = Result.Success(clienteActualizado);
        return successResult.ToHttpResult();
    }
}