using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public class ObtenerClientePorIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/clientes/{id:int}", Manejador)
           .WithName("ObtenerClientePorId")
           .WithTags("Clientes")
           .WithSummary("Obtiene el detalle de un cliente por su ID");
    }

    private static IResult Manejador(int id)
    {
        var cliente = ClienteDataStore.ClientesDb.FirstOrDefault(cliente => cliente.Id == id);
        if (cliente is null)
        {
            Result<ClienteResponse> errorResult = Error.NotFound(
                "Cliente.NotFound",
                $"No se encontro un cliente con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var response = new ClienteResponse(
            Id: cliente.Id,
            Nombre: cliente.Nombre,
            CedulaNit: cliente.Documento.Value,
            Telefono: cliente.Telefono.Valor,
            Direccion: cliente.Direccion,
            TipoNegocio: cliente.TipoNegocio
        );

        Result<ClienteResponse> successResult = Result.Success(response);
        return successResult.ToHttpResult();
    }
}