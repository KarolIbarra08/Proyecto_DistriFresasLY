using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        int id,
        IRolRepository rolRepository,
        CancellationToken cancellationToken)
    {
        var rol = await rolRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (rol is null)
        {
            return Result.Failure<string>(
                Error.NotFound("Rol.NotFound", $"No se encontró un rol con el Id: {id}")
            ).ToHttpResult();
        }

        await rolRepository.EliminarAsync(id, cancellationToken);

        return Result.Success($"El rol con ID {id} se eliminó correctamente.").ToHttpResult();
    }
}