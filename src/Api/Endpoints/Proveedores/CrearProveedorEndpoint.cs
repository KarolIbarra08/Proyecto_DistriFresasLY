using DistriFresasLY.Api.Contracts.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        CrearProveedorRequest request,
        IProveedorRepository proveedorRepository,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.CedulaNit))
        {
            Result<ProveedorResponse> validationResult = Error.Validation(
                "Proveedor.Validacion",
                "El nombre y la Cedula/NIT son campos obligatorios.");

            return validationResult.ToHttpResult();
        }

        var proveedorExistente = await proveedorRepository.ObtenerPorCedulaNitAsync(request.CedulaNit, cancellationToken);
        if (proveedorExistente is not null)
        {
            Result<ProveedorResponse> conflictResult = Error.Conflict(
                "Proveedor.CedulaDuplicada",
                $"Ya existe un proveedor registrado con la Cedula/NIT '{request.CedulaNit}'.");

            return conflictResult.ToHttpResult();
        }

        var proveedorResult = Proveedor.Create(
            nombre: request.Nombre,
            apellido: request.Apellido,
            cedulaNit: request.CedulaNit,
            telefono: request.Telefono,
            direccion: request.Direccion,
            nombreEmpresa: request.NombreEmpresa);

        if (proveedorResult.IsFailure)
        {
            return Result.Failure<ProveedorResponse>(proveedorResult.Error).ToHttpResult();
        }

        var proveedor = proveedorResult.Value;
        var nuevoId = await proveedorRepository.AgregarAsync(proveedor, cancellationToken);

        var response = new ProveedorResponse(
            Id: nuevoId,
            Nombre: proveedor.Nombre,
            Apellido: proveedor.Apellido,
            CedulaNit: proveedor.Documento.Value,
            Telefono: proveedor.Telefono.Valor,
            Direccion: proveedor.Direccion,
            NombreEmpresa: proveedor.NombreEmpresa
        );

        Result<ProveedorResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/proveedores/{nuevoId}");
    }
}