using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Roles;

public class ObtenerRolesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/roles", Manejador)
           .WithName("ObtenerRoles")
           .WithTags("Roles")
           .WithSummary("Obtiene el listado de roles del sistema");
    }

    private static async Task<IResult> Manejador(
        IRolRepository rolRepository,
        CancellationToken cancellationToken)
    {
        var roles = await rolRepository.ObtenerTodosAsync(cancellationToken);

        var lista = roles.Select(r => new RolResponse(
            r.Id,
            r.Nombre,
            r.Descripcion,
            r.Permisos.ToList()
        )).ToList();

        return Result.Success(lista).ToHttpResult();
    }
}