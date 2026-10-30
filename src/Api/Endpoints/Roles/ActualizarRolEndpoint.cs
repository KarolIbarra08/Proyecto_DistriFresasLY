using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        int id,
        ActualizarRolRequest request,
        IRolRepository rolRepository,
        CancellationToken cancellationToken)
    {
        var rol = await rolRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (rol is null)
        {
            return Result.Failure<RolResponse>(
                Error.NotFound("Rol.NotFound", $"No se encontró un rol con el Id: {id}")
            ).ToHttpResult();
        }

        var nombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre.Trim() : rol.Nombre;
        var descripcion = request.Descripcion != null ? request.Descripcion.Trim() : rol.Descripcion;

        var actualizarResult = rol.Actualizar(nombre, descripcion);
        if (actualizarResult.IsFailure)
        {
            return Result.Failure<RolResponse>(actualizarResult.Error).ToHttpResult();
        }

        await rolRepository.ActualizarAsync(rol, cancellationToken);

        var response = new RolResponse(
            rol.Id,
            rol.Nombre,
            rol.Descripcion,
            rol.Permisos.ToList()
        );

        return Result.Success(response).ToHttpResult();
    }
}