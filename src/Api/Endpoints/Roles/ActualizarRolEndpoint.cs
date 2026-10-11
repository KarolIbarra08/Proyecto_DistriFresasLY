using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Roles;

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
                $"No se encontro un rol con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var rol = RolDataStore.RolesDb[index];

        var nombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre.Trim() : rol.Nombre;
        var descripcion = request.Descripcion != null ? request.Descripcion.Trim() : rol.Descripcion;

        var actualizarResult = rol.Actualizar(nombre, descripcion);
        if (actualizarResult.IsFailure)
        {
            return Result.Failure<RolResponse>(actualizarResult.Error).ToHttpResult();
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