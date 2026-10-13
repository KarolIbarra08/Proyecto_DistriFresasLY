using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Roles;

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

        
        var rolResult = Rol.Create(
            nombre: request.Nombre,
            descripcion: request.Descripcion,
            id: nuevoId);

        if (rolResult.IsFailure)
        {
            return Result.Failure<RolResponse>(rolResult.Error).ToHttpResult();
        }

        var rol = rolResult.Value;
        RolDataStore.RolesDb.Add(rol);

  
        var response = new RolResponse(
            rol.Id,
            rol.Nombre,
            rol.Descripcion,
            rol.Permisos.ToList()
        );

        Result<RolResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/roles/{rol.Id}");
    }
}