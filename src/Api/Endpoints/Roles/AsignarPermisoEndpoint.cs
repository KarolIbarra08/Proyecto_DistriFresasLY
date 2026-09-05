using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

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
                $"No se encontró un rol con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var rolExistente = RolDataStore.RolesDb[index];
        var permisoNormalizado = request.Permiso.Trim().ToLower();

        if (!rolExistente.Permisos.Contains(permisoNormalizado))
        {
            rolExistente.Permisos.Add(permisoNormalizado);
        }

        var response = new RolResponse(
            rolExistente.Id,
            rolExistente.Nombre,
            rolExistente.Descripcion,
            rolExistente.Permisos
        );

        return Result.Success(response).ToHttpResult();
    }
}