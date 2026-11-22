using DistriFresasLY.Api.Contracts.Roles;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Roles;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Roles;

public class RolEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/roles")
                       .WithTags("Roles");

        group.MapGet("/", ObtenerTodos)
             .WithName("ObtenerRoles")
             .WithSummary("Obtiene el listado de roles del sistema");

        group.MapPost("/", Crear)
             .WithName("CrearRol")
             .WithSummary("Registra un nuevo rol en el sistema");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarRol")
             .WithSummary("Actualiza los datos de un rol existente");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarRol")
             .WithSummary("Elimina un rol del sistema por su ID");

        group.MapPost("/{id:int}/permisos", AsignarPermiso)
             .WithName("AsignarPermiso")
             .WithSummary("Asigna un nuevo permiso a un rol específico");

        group.MapPost("/{id:int}/verificar-acceso", VerificarAcceso)
             .WithName("VerificarAcceso")
             .WithSummary("Valida si un rol cuenta con un permiso específico");
    }

    private static async Task<IResult> ObtenerTodos(
        ObtenerRolesUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<List<RolResponse>>(resultado.Error).ToHttpResult();
        }

        var lista = resultado.Value.Select(r => r.ToResponse()).ToList();
        return Result.Success(lista).ToHttpResult();
    }

    private static async Task<IResult> Crear(
        CrearRolRequest request,
        CrearRolUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<RolResponse>(resultado.Error).ToHttpResult();
        }

        var response = resultado.Value.ToResponse();
        return Result.Success(response).ToHttpCreatedAtResult($"/api/roles/{response.Id}");
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarRolRequest request,
        ActualizarRolUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<RolResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarRolUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<string>(resultado.Error).ToHttpResult();
        }

        return Result.Success($"El rol con ID {id} se eliminó correctamente.").ToHttpResult();
    }

    private static async Task<IResult> AsignarPermiso(
        int id,
        AsignarPermisoRequest request,
        AsignarPermisoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<RolResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> VerificarAcceso(
        int id,
        VerificarAccesoRequest request,
        VerificarAccesoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToQuery(id), cancellationToken);
        return resultado.ToHttpResult();
    }
}