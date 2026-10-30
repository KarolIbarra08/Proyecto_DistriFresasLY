using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Cliente;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public class ActualizarClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/clientes/{id:int}", Manejador)
           .WithName("ActualizarCliente")
           .WithTags("Clientes")
           .WithSummary("Actualiza la información de un cliente existente");
    }

    private static async Task<IResult> Manejador(
        int id,
        ActualizarClienteRequest request,
        IClienteRepository clienteRepository,
        CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            return Result.Failure<ClienteResponse>(
                Error.NotFound("Cliente.NotFound", $"No se encontró el cliente con ID {id}")
            ).ToHttpResult();
        }

        if (!string.IsNullOrWhiteSpace(request.CedulaNit) && cliente.Documento.Value != request.CedulaNit)
        {
            var existeDocumento = await clienteRepository.ExistePorDocumentoAsync(
                request.CedulaNit, 
                id, 
                cancellationToken);

            if (existeDocumento)
            {
                return Result.Failure<ClienteResponse>(
                    Error.Conflict("Cliente.CedulaNitDuplicada", "La Cédula/NIT ya está registrada por otro cliente.")
                ).ToHttpResult();
            }

            var docResult = cliente.ActualizarDocumento(request.CedulaNit);
            if (docResult.IsFailure)
            {
                return Result.Failure<ClienteResponse>(docResult.Error).ToHttpResult();
            }
        }


        var updateResult = cliente.ActualizarDatos(
            request.Nombre,
            request.Telefono,
            request.Direccion,
            request.TipoNegocio);

        if (updateResult.IsFailure)
        {
            return Result.Failure<ClienteResponse>(updateResult.Error).ToHttpResult();
        }

        await clienteRepository.ActualizarAsync(cliente, cancellationToken);

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