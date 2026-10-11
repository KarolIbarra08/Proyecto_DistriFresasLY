using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
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

        var cliente = ClienteDataStore.ClientesDb[index];

        if (!string.IsNullOrWhiteSpace(request.CedulaNit))
        {
            var documentoResult = cliente.ActualizarDocumento(request.CedulaNit.Trim());
            if (documentoResult.IsFailure)
            {
                return Result.Failure<ClienteResponse>(documentoResult.Error).ToHttpResult();
            }
        }

        var nombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre.Trim() : cliente.Nombre;
        var telefono = !string.IsNullOrWhiteSpace(request.Telefono) ? request.Telefono.Trim() : cliente.Telefono.Valor;
        var direccion = !string.IsNullOrWhiteSpace(request.Direccion) ? request.Direccion.Trim() : cliente.Direccion;
        var tipoNegocio = !string.IsNullOrWhiteSpace(request.TipoNegocio) ? request.TipoNegocio.Trim() : cliente.TipoNegocio;

        var actualizarResult = cliente.ActualizarDatos(nombre, telefono, direccion, tipoNegocio);
        if (actualizarResult.IsFailure)
        {
            return Result.Failure<ClienteResponse>(actualizarResult.Error).ToHttpResult();
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