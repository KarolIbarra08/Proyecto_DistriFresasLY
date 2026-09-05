using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Roles;

public class ActualizarRolEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/roles/{id:int}", Manejador)
           .WithName("ActualizarRol")
           .WithTags("Roles")
           .WithSummary("Actualiza los datos de un rol existente");
    }

    private static IResult Manejador(int id, ActualizarRolRequest request)
    {
        var index = RolDataStore.RolesDb.FindIndex(r => r.Id == id);
        if (index == -1)
        {
            Result<RolResponse> errorResult = Error.NotFound(
                "Rol.NotFound",
                $"No se encontró un rol con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var rolExistente = RolDataStore.RolesDb[index];

       
        var rolActualizado = rolExistente with
        {
            Nombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre.Trim() : rolExistente.Nombre,
            Descripcion = request.Descripcion != null ? request.Descripcion.Trim() : rolExistente.Descripcion
        };

        RolDataStore.RolesDb[index] = rolActualizado;

        var response = new RolResponse(
            rolActualizado.Id,
            rolActualizado.Nombre,
            rolActualizado.Descripcion,
            rolActualizado.Permisos
        );

        return Result.Success(response).ToHttpResult();
    }
}