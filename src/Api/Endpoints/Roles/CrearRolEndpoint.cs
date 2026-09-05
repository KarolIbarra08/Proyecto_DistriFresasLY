using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Roles;

public class CrearRolEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/roles", Manejador)
           .WithName("CrearRol")
           .WithTags("Roles")
           .WithSummary("Registra un nuevo rol en el sistema");
    }

    private static IResult Manejador(CrearRolRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            Result<RolResponse> validationResult = Error.Validation(
                "Rol.Validacion",
                "El nombre del rol es obligatorio.");

            return validationResult.ToHttpResult();
        }

        if (RolDataStore.RolesDb.Any(r => r.Nombre.Equals(request.Nombre, StringComparison.OrdinalIgnoreCase)))
        {
            Result<RolResponse> conflictResult = Error.Conflict(
                "Rol.Duplicado",
                $"El rol '{request.Nombre}' ya se encuentra registrado.");

            return conflictResult.ToHttpResult();
        }

        var nuevoId = RolDataStore.RolesDb.Count != 0 
            ? RolDataStore.RolesDb.Max(r => r.Id) + 1 
            : 1;

        var nuevoRol = new RolModel(
            Id: nuevoId,
            Nombre: request.Nombre.Trim(),
            Descripcion: request.Descripcion?.Trim() ?? string.Empty,
            Permisos: []
        );

        RolDataStore.RolesDb.Add(nuevoRol);

        var response = new RolResponse(
            nuevoRol.Id,
            nuevoRol.Nombre,
            nuevoRol.Descripcion,
            nuevoRol.Permisos
        );

        Result<RolResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/roles/{nuevoRol.Id}");
    }
}