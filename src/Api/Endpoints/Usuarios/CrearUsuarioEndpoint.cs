using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Usuarios;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class CrearUsuarioEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/usuarios", Manejador)
           .WithName("CrearUsuario")
           .WithTags("Usuarios")
           .WithSummary("Registra un nuevo usuario en el sistema");
    }

    private static IResult Manejador(CrearUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Contrasena))
        {
            Result<UsuarioResponse> validationResult = Error.Validation(
                "Usuario.Validacion",
                "El nombre de usuario y la contraseña son obligatorios.");

            return validationResult.ToHttpResult();
        }

        if (UsuarioDataStore.UsuariosDb.Any(u => u.NombreUsuario.Equals(request.Usuario, StringComparison.OrdinalIgnoreCase)))
        {
            Result<UsuarioResponse> conflictResult = Error.Conflict(
                "Usuario.Duplicado",
                $"El usuario '{request.Usuario}' ya se encuentra registrado.");

            return conflictResult.ToHttpResult();
        }

        var nuevoId = UsuarioDataStore.UsuariosDb.Count != 0
            ? UsuarioDataStore.UsuariosDb.Max(u => u.Id) + 1
            : 1;

        var usuarioResult = Usuario.Create(
            nombre: request.Nombre,
            apellido: request.Apellido,
            usuario: request.Usuario,
            contrasena: request.Contrasena,
            rol: string.IsNullOrWhiteSpace(request.Rol) ? "Usuario" : request.Rol,
            id: nuevoId);

        if (usuarioResult.IsFailure)
        {
            return Result.Failure<UsuarioResponse>(usuarioResult.Error).ToHttpResult();
        }

        var usuario = usuarioResult.Value;
        UsuarioDataStore.UsuariosDb.Add(usuario);

        var response = new UsuarioResponse(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.NombreUsuario,
            usuario.Contrasena,
            usuario.Rol,
            usuario.Estado
        );

        Result<UsuarioResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/usuarios/{usuario.Id}");
    }
}