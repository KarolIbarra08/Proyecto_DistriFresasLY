using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Roles;

public class EliminarRolEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/roles/{id:int}", Manejador)
           .WithName("EliminarRol")
           .WithTags("Roles")
           .WithSummary("Elimina un rol del sistema por su ID");
    }

    private static IResult Manejador(int id)
    {
        var index = RolDataStore.RolesDb.FindIndex(r => r.Id == id);
        if (index == -1)
        {
            Result<bool> errorResult = Error.NotFound(
                "Rol.NotFound",
                $"No se encontró un rol con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        RolDataStore.RolesDb.RemoveAt(index);

        Result<bool> successResult = Result.Success(true);
        return successResult.ToHttpResult();
    }
}