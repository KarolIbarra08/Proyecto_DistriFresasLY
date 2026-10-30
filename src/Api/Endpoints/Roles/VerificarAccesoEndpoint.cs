using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Roles;

public class VerificarAccesoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/roles/{id:int}/verificar-acceso", Manejador)
           .WithName("VerificarAcceso")
           .WithTags("Roles")
           .WithSummary("Valida si un rol cuenta con un permiso específico");
    }

    private static async Task<IResult> Manejador(
        int id,
        VerificarAccesoRequest request,
        IRolRepository rolRepository,
        CancellationToken cancellationToken)
    {
        var rol = await rolRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (rol is null)
        {
            return Result.Failure<bool>(
                Error.NotFound("Rol.NotFound", $"No se encontró un rol con el Id: {id}")
            ).ToHttpResult();
        }

        var tieneAcceso = rol.TieneAcceso(request.Permiso);

        if (!tieneAcceso)
        {
            return Result.Failure<bool>(
                Error.Validation(
                    "Rol.AccesoDenegado",
                    $"El rol '{rol.Nombre}' no cuenta con el permiso '{request.Permiso}'."))
                .ToHttpResult();
        }

        return Result.Success(true).ToHttpResult();
    }
}