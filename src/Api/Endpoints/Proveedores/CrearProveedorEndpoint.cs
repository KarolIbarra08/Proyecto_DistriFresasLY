using DistriFresasLY.Api.Contracts.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Proveedores;

public class CrearProveedorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/proveedores", Manejador)
           .WithName("CrearProveedor")
           .WithTags("Proveedores")
           .WithSummary("Registra un nuevo proveedor");
    }

    private static IResult Manejador(CrearProveedorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.CedulaNit))
        {
            Result<ProveedorResponse> validationResult = Error.Validation(
                "Proveedor.Validacion",
                "El nombre y la Cedula/NIT son campos obligatorios.");

            return validationResult.ToHttpResult();
        }

        if (ProveedorDataStore.ProveedoresDb.Any(proveedor => proveedor.CedulaNit.Equals(request.CedulaNit, StringComparison.OrdinalIgnoreCase)))
        {
            Result<ProveedorResponse> conflictResult = Error.Conflict(
                "Proveedor.CedulaDuplicada",
                $"Ya existe un proveedor registrado con la Cedula/NIT '{request.CedulaNit}'.");

            return conflictResult.ToHttpResult();
        }

        var nuevoId = ProveedorDataStore.ProveedoresDb.Count != 0 ? ProveedorDataStore.ProveedoresDb.Max(proveedor => proveedor.Id) + 1 : 1;

        var nuevoProveedor = new ProveedorResponse(
            Id: nuevoId,
            Nombre: request.Nombre.Trim(),
            Apellido:request.Apellido?.Trim() ?? string.Empty,
            CedulaNit: request.CedulaNit.Trim(),
            Telefono: request.Telefono?.Trim() ?? string.Empty,
            Direccion: request.Direccion?.Trim() ?? string.Empty,
            NombreEmpresa : request.NombreEmpresa?.Trim() ?? string.Empty
        );

        ProveedorDataStore.ProveedoresDb.Add(nuevoProveedor);

        Result<ProveedorResponse> createdResult = Result.Success(nuevoProveedor);
        return createdResult.ToHttpCreatedAtResult($"/api/proveedores/{nuevoProveedor.Id}");
    }
}