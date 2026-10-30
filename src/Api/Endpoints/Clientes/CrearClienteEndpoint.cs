using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Cliente; 

namespace DistriFresasLY.Api.Endpoints.Clientes;

public class CrearClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/clientes", Manejador)
           .WithName("CrearCliente")
           .WithTags("Clientes")
           .WithSummary("Crea un nuevo cliente en el sistema");
    }

    private static async Task<IResult> Manejador(
        CrearClienteRequest request,
        IClienteRepository clienteRepository,
        CancellationToken cancellationToken)
    {
        var existeDocumento = await clienteRepository.ExistePorDocumentoAsync(
            request.CedulaNit, 
            cancellationToken: cancellationToken);

        if (existeDocumento)
        {
            return Result.Failure<ClienteResponse>(
                Error.Conflict("Cliente.CedulaNitDuplicada", "La Cédula/NIT ya está registrada.")
            ).ToHttpResult();
        }

      
        var clienteResult = Cliente.Create(
            request.Nombre,
            request.CedulaNit,
            request.Telefono,
            request.Direccion,
            request.TipoNegocio
        );

        if (clienteResult.IsFailure)
        {
            return Result.Failure<ClienteResponse>(clienteResult.Error).ToHttpResult();
        }

        var idGenerado = await clienteRepository.AgregarAsync(clienteResult.Value, cancellationToken);

        var clienteCreado = await clienteRepository.ObtenerPorIdAsync(idGenerado, cancellationToken);

        var response = new ClienteResponse(
            Id: clienteCreado!.Id,
            Nombre: clienteCreado.Nombre,
            CedulaNit: clienteCreado.Documento.Value,
            Telefono: clienteCreado.Telefono.Valor,
            Direccion: clienteCreado.Direccion,
            TipoNegocio: clienteCreado.TipoNegocio
        );

        return Result.Success(response).ToHttpResult();
    }
}