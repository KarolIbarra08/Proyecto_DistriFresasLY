using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public class CrearClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/clientes", Manejador)
           .WithName("CrearCliente")
           .WithTags("Clientes")
           .WithSummary("Registra un nuevo cliente");
    }

    private static IResult Manejador(CrearClienteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.CedulaNit))
        {
            Result<ClienteResponse> validationResult = Error.Validation(
                "Cliente.Validacion",
                "El nombre y la Cedula/NIT son campos obligatorios.");

            return validationResult.ToHttpResult();
        }

        if (ClienteDataStore.ClientesDb.Any(cliente => cliente.CedulaNit.Equals(request.CedulaNit, StringComparison.OrdinalIgnoreCase)))
        {
            Result<ClienteResponse> conflictResult = Error.Conflict(
                "Cliente.CedulaDuplicada",
                $"Ya existe un cliente registrado con la Cedula/NIT '{request.CedulaNit}'.");

            return conflictResult.ToHttpResult();
        }

        var nuevoId = ClienteDataStore.ClientesDb.Count != 0 ? ClienteDataStore.ClientesDb.Max(cliente => cliente.Id) + 1 : 1;

        var nuevoCliente = new ClienteResponse(
            Id: nuevoId,
            Nombre: request.Nombre.Trim(),
            CedulaNit: request.CedulaNit.Trim(),
            Telefono: request.Telefono?.Trim() ?? string.Empty,
            Direccion: request.Direccion?.Trim() ?? string.Empty,
            TipoNegocio: request.TipoNegocio?.Trim() ?? string.Empty
        );

        ClienteDataStore.ClientesDb.Add(nuevoCliente);

        Result<ClienteResponse> createdResult = Result.Success(nuevoCliente);
        return createdResult.ToHttpCreatedAtResult($"/api/clientes/{nuevoCliente.Id}");
    }
}