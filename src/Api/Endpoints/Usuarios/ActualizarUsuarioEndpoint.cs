using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class ActualizarUsuarioEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/usuarios/{id:int}", Manejador)
           .WithName("ActualizarUsuario")
           .WithTags("Usuarios")
           .WithSummary("Actualiza los datos de un usuario existente");
    }

    private static IResult Manejador(int id, ActualizarUsuarioRequest request)
    {
        var index = UsuarioDataStore.UsuariosDb.FindIndex(u => u.Id == id);
        if (index == -1)
        {
            Result<UsuarioResponse> errorResult = Error.NotFound(
                "Usuario.NotFound", 
                $"No se encontro un usuario con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var usuarioExistente = UsuarioDataStore.UsuariosDb[index];

        var usuarioActualizado = usuarioExistente with
        {
            Nombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre.Trim() : usuarioExistente.Nombre,
            Apellido = !string.IsNullOrWhiteSpace(request.Apellido) ? request.Apellido.Trim() : usuarioExistente.Apellido,
            Usuario = !string.IsNullOrWhiteSpace(request.Usuario) ? request.Usuario.Trim() : usuarioExistente.Usuario,
            Contrasena = !string.IsNullOrWhiteSpace(request.Contrasena) ? request.Contrasena : usuarioExistente.Contrasena,
            Rol = !string.IsNullOrWhiteSpace(request.Rol) ? request.Rol.Trim() : usuarioExistente.Rol
        };

        UsuarioDataStore.UsuariosDb[index] = usuarioActualizado;

        var response = new UsuarioResponse(
            usuarioActualizado.Id, 
            usuarioActualizado.Nombre, 
            usuarioActualizado.Apellido, 
            usuarioActualizado.Usuario, 
            usuarioActualizado.Contrasena,
            usuarioActualizado.Rol, 
            usuarioActualizado.Estado);

        return Result.Success(response).ToHttpResult();
    }
}