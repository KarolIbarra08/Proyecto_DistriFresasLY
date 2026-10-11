using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Roles;

namespace DistriFresasLY.Api.Endpoints.Roles;

public class AsignarPermisoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/roles/{id:int}/permisos", Manejador)
           .WithName("AsignarPermiso")
           .WithTags("Roles")
           .WithSummary("Asigna un nuevo permiso a un rol especifico");
    }

    private static IResult Manejador(int id, AsignarPermisoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Permiso))
        {
            Result<RolResponse> validationResult = Error.Validation(
                "Rol.PermisoVacio",
                "El nombre del permiso es obligatorio.");

            return validationResult.ToHttpResult();
        }

        var index = RolDataStore.RolesDb.FindIndex(r => r.Id == id);
        if (index == -1)
        {
            Result<RolResponse> errorResult = Error.NotFound(
                "Rol.NotFound",
                $"No se encontro un rol con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var rol = RolDataStore.RolesDb[index];

        var asignarResult = rol.AgregarPermiso(request.Permiso);
        if (asignarResult.IsFailure)
        {
            return Result.Failure<RolResponse>(asignarResult.Error).ToHttpResult();
        }

        var response = new RolResponse(
            rol.Id,
            rol.Nombre,
            rol.Descripcion,
            rol.Permisos.ToList()
        );

        return Result.Success(response).ToHttpResult();
    }
}