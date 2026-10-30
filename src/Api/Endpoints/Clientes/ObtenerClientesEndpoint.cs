using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        IClienteRepository clienteRepository,
        CancellationToken cancellationToken)
    {
        var clientes = await clienteRepository.ObtenerTodosAsync(cancellationToken);

        // Se evalúa la proyección a un arreglo para que el serializador JSON pueda transmitir la lista completa
        var response = clientes.Select(cliente => new ClienteResponse(
            Id: cliente.Id,
            Nombre: cliente.Nombre,
            CedulaNit: cliente.Documento.Value,
            Telefono: cliente.Telefono.Valor,
            Direccion: cliente.Direccion,
            TipoNegocio: cliente.TipoNegocio
        )).ToArray();

        return Result.Success(response).ToHttpResult();
    }
}