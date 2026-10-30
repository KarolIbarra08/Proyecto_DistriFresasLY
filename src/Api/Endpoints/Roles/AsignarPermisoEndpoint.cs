using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Roles;

public class AsignarPermisoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/roles/{id:int}/permisos", Manejador)
           .WithName("AsignarPermiso")
           .WithTags("Roles")
           .WithSummary("Asigna un nuevo permiso a un rol específico");
    }

    private static async Task<IResult> Manejador(
        int id,
        AsignarPermisoRequest request,
        IRolRepository rolRepository,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Permiso))
        {
            return Result.Failure<RolResponse>(
                Error.Validation("Rol.PermisoVacio", "El nombre del permiso es obligatorio.")
            ).ToHttpResult();
        }

        var rol = await rolRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (rol is null)
        {
            return Result.Failure<RolResponse>(
                Error.NotFound("Rol.NotFound", $"No se encontró un rol con el Id: {id}")
            ).ToHttpResult();
        }

        var asignarResult = rol.AgregarPermiso(request.Permiso);
        if (asignarResult.IsFailure)
        {
            return Result.Failure<RolResponse>(asignarResult.Error).ToHttpResult();
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