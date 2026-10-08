using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
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
           .WithSummary("Registra un nuevo cliente");
    }

    private static IResult Manejador(CrearClienteRequest request)
    {
        // Verificar si ya existe un cliente con la misma Cedula/NIT
        if (ClienteDataStore.ClientesDb.Any(
            cliente => cliente.CedulaNit.Equals(
                request.CedulaNit,
                StringComparison.OrdinalIgnoreCase)))
        {
            Result<ClienteResponse> conflictResult = Error.Conflict(
                "Cliente.CedulaDuplicada",
                $"Ya existe un cliente registrado con la Cédula/NIT '{request.CedulaNit}'.");

            return conflictResult.ToHttpResult();
        }

        // Generar el identificador del nuevo cliente
        var nuevoId = ClienteDataStore.ClientesDb.Count != 0
            ? ClienteDataStore.ClientesDb.Max(cliente => cliente.Id) + 1
            : 1;


        var clienteResult = Cliente.Create(
            nombre: request.Nombre,
            cedulaNit: request.CedulaNit,
            telefono: request.Telefono,
            direccion: request.Direccion,
            tipoNegocio: request.TipoNegocio,
            id: nuevoId);

   
        if (clienteResult.IsFailure)
        {
            return clienteResult.ToHttpResult();
        }

        var cliente = clienteResult.Value;

        // Crear la respuesta de la API
        var response = new ClienteResponse(
            Id: cliente.Id,
            Nombre: cliente.Nombre,
            CedulaNit: cliente.Documento.Value,
            Telefono: cliente.Telefono.Valor,
            Direccion: cliente.Direccion,
            TipoNegocio: cliente.TipoNegocio
        );

        // Guardar el cliente
        ClienteDataStore.ClientesDb.Add(response);

        Result<ClienteResponse> createdResult = Result.Success(response);

        return createdResult.ToHttpCreatedAtResult(
            $"/api/clientes/{cliente.Id}");
    }
}