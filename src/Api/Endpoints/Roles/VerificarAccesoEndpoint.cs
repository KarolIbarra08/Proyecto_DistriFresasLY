using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Roles;

public class VerificarAccesoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/roles/{id:int}/verificar-acceso", Manejador)
           .WithName("VerificarAcceso")
           .WithTags("Roles")
           .WithSummary("Valida si un rol cuenta con un permiso especifico");
    }

    private static IResult Manejador(int id, VerificarAccesoRequest request)
    {
        var rol = RolDataStore.RolesDb.FirstOrDefault(r => r.Id == id);
        if (rol is null)
        {
            Result<bool> errorResult = Error.NotFound(
                "Rol.NotFound",
                $"No se encontró un rol con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var tieneAcceso = rol.Permisos.Contains(request.Permiso.Trim().ToLower());

        if (!tieneAcceso)
        {
            Result<bool> forbiddenResult = Error.Validation(
                "Rol.AccesoDenegado",
                $"El rol '{rol.Nombre}' no cuenta con el permiso '{request.Permiso}'.");

            return forbiddenResult.ToHttpResult();
        }

        return Result.Success(true).ToHttpResult();
    }
}