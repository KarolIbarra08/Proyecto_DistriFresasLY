using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Roles;

namespace DistriFresasLY.Api.Endpoints.Roles;

public class CrearRolEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/roles", Manejador)
           .WithName("CrearRol")
           .WithTags("Roles")
           .WithSummary("Registra un nuevo rol en el sistema");
    }

    private static async Task<IResult> Manejador(
        CrearRolRequest request,
        IRolRepository rolRepository,
        CancellationToken cancellationToken)
    {
        var rolExistente = await rolRepository.ObtenerPorNombreAsync(request.Nombre, cancellationToken);
        if (rolExistente is not null)
        {
            return Result.Failure<RolResponse>(
                Error.Conflict(
                    "Rol.Duplicado",
                    $"El rol '{request.Nombre}' ya se encuentra registrado."))
                .ToHttpResult();
        }

        var rolResult = Rol.Create(
            nombre: request.Nombre,
            descripcion: request.Descripcion,
            id: 0);

        if (rolResult.IsFailure)
        {
            return Result.Failure<RolResponse>(rolResult.Error).ToHttpResult();
        }

        var rol = rolResult.Value;

        int nuevoId = await rolRepository.AgregarAsync(rol, cancellationToken);

        var response = new RolResponse(
            nuevoId,
            rol.Nombre,
            rol.Descripcion,
            rol.Permisos.ToList()
        );

        return Result.Success(response)
            .ToHttpCreatedAtResult($"/api/roles/{nuevoId}");
    }
}