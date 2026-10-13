using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Endpoints.Roles;
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
       
        if (UsuarioDataStore.UsuariosDb.Any(u => u.NombreUsuario.Equals(request.Usuario, StringComparison.OrdinalIgnoreCase)))
        {
            Result<UsuarioResponse> conflictResult = Error.Conflict(
                "Usuario.Duplicado",
                $"El usuario '{request.Usuario}' ya se encuentra registrado.");

            return conflictResult.ToHttpResult();
        }

     
        var rolNombre = string.IsNullOrWhiteSpace(request.Rol) ? "Usuario" : request.Rol;
        bool existeRol = RolDataStore.RolesDb.Any(r => r.Nombre.Equals(rolNombre, StringComparison.OrdinalIgnoreCase));

        if (!existeRol)
        {
            Result<UsuarioResponse> rolNotFoundResult = Error.NotFound(
                "Usuario.RolNoEncontrado",
                $"El rol '{rolNombre}' no existe.");

            return rolNotFoundResult.ToHttpResult();
        }


        var nuevoId = UsuarioDataStore.UsuariosDb.Count != 0
            ? UsuarioDataStore.UsuariosDb.Max(u => u.Id) + 1
            : 1;


        var usuarioResult = Usuario.Create(
            nombre: request.Nombre,
            apellido: request.Apellido,
            usuario: request.Usuario,
            contrasena: request.Contrasena,
            rol: rolNombre,
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