using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
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

    private static IResult Manejador()
    {
        var lista = RolDataStore.RolesDb.Select(r => new RolResponse(
            r.Id,
            r.Nombre,
            r.Descripcion,
            r.Permisos
        )).ToList();

        Result<List<RolResponse>> successResult = Result.Success(lista);
        return successResult.ToHttpResult();
    }
}