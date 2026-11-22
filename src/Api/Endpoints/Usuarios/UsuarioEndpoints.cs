using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Usuarios;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class UsuarioEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/usuarios")
                       .WithTags("Usuarios");

        group.MapGet("/", ObtenerTodos)
             .WithName("ObtenerUsuarios")
             .WithSummary("Obtiene la lista de todos los usuarios registrados");

        group.MapGet("/{id:int}", ObtenerPorId)
             .WithName("ObtenerUsuarioPorId")
             .WithSummary("Obtiene un usuario por su ID");

        group.MapPost("/", Crear)
             .WithName("CrearUsuario")
             .WithSummary("Crea un nuevo usuario en el sistema");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarUsuario")
             .WithSummary("Actualiza los datos de un usuario existente");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarUsuario")
             .WithSummary("Elimina un usuario del sistema por su ID");

        group.MapPost("/login", IniciarSesion)
             .WithName("IniciarSesion")
             .WithSummary("Inicia sesión en el sistema");

        group.MapPost("/logout", CerrarSesion)
             .WithName("CerrarSesion")
             .WithSummary("Cierra la sesion del usuario actual");
    }

    private static async Task<IResult> ObtenerTodos(
        ObtenerUsuariosUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<List<UsuarioResponse>>(resultado.Error).ToHttpResult();
        }

        var listaUsuarios = resultado.Value.Select(u => u.ToResponse()).ToList();
        return Result.Success(listaUsuarios).ToHttpResult();
    }

    private static async Task<IResult> ObtenerPorId(
        int id,
        ObtenerUsuarioPorIdUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<UsuarioResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Crear(
        CrearUsuarioRequest request,
        CrearUsuarioUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<UsuarioResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarUsuarioRequest request,
        ActualizarUsuarioUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<UsuarioResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarUsuarioUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<string>(resultado.Error).ToHttpResult();
        }

        return Result.Success($"Usuario con Id {id} eliminado correctamente.").ToHttpResult();
    }

    private static async Task<IResult> IniciarSesion(
        LoginRequest request,
        IniciarSesionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<UsuarioResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static IResult CerrarSesion()
    {
        return Result.Success("Sesion cerrada correctamente.").ToHttpResult();
    }
}