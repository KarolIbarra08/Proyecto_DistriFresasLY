using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        int id,
        IClienteRepository clienteRepository,
        CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.ObtenerPorIdAsync(id, cancellationToken);
        
        if (cliente is null)
        {
            var errorResult = Error.NotFound(
                "Cliente.NotFound",
                $"No se encontro un cliente con el Id: {id}");

            return Result.Failure<ClienteResponse>(errorResult).ToHttpResult();
        }

        var response = new ClienteResponse(
            Id: cliente.Id,
            Nombre: cliente.Nombre,
            CedulaNit: cliente.Documento.Value,
            Telefono: cliente.Telefono.Valor,
            Direccion: cliente.Direccion,
            TipoNegocio: cliente.TipoNegocio
        );

        return Result.Success(response).ToHttpResult();
    }
}