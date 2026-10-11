using DistriFresasLY.Api.Contracts.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Proveedores;

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

        if (ProveedorDataStore.ProveedoresDb.Any(proveedor => proveedor.Documento.Value.Equals(request.CedulaNit, StringComparison.OrdinalIgnoreCase)))
        {
            Result<ProveedorResponse> conflictResult = Error.Conflict(
                "Proveedor.CedulaDuplicada",
                $"Ya existe un proveedor registrado con la Cedula/NIT '{request.CedulaNit}'.");

            return conflictResult.ToHttpResult();
        }

        var nuevoId = ProveedorDataStore.ProveedoresDb.Count != 0 ? ProveedorDataStore.ProveedoresDb.Max(proveedor => proveedor.Id) + 1 : 1;

        var proveedorResult = Proveedor.Create(
            nombre: request.Nombre,
            apellido: request.Apellido,
            cedulaNit: request.CedulaNit,
            telefono: request.Telefono,
            direccion: request.Direccion,
            nombreEmpresa: request.NombreEmpresa,
            id: nuevoId);

        if (proveedorResult.IsFailure)
        {
            return Result.Failure<ProveedorResponse>(proveedorResult.Error).ToHttpResult();
        }

        var proveedor = proveedorResult.Value;
        ProveedorDataStore.ProveedoresDb.Add(proveedor);

        var response = new ProveedorResponse(
            Id: proveedor.Id,
            Nombre: proveedor.Nombre,
            Apellido: proveedor.Apellido,
            CedulaNit: proveedor.Documento.Value,
            Telefono: proveedor.Telefono.Valor,
            Direccion: proveedor.Direccion,
            NombreEmpresa: proveedor.NombreEmpresa
        );

        Result<ProveedorResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/proveedores/{proveedor.Id}");
    }
}