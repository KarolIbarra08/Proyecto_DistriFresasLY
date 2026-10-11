using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Usuarios;

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

        var usuario = UsuarioDataStore.UsuariosDb[index];

        var nombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre.Trim() : usuario.Nombre;
        var apellido = !string.IsNullOrWhiteSpace(request.Apellido) ? request.Apellido.Trim() : usuario.Apellido;
        var nombreUsuario = !string.IsNullOrWhiteSpace(request.Usuario) ? request.Usuario.Trim() : usuario.NombreUsuario;
        var contrasena = !string.IsNullOrWhiteSpace(request.Contrasena) ? request.Contrasena : usuario.Contrasena;
        var rol = !string.IsNullOrWhiteSpace(request.Rol) ? request.Rol.Trim() : usuario.Rol;

        var actualizarResult = usuario.ActualizarDatos(nombre, apellido, nombreUsuario, contrasena, rol);
        if (actualizarResult.IsFailure)
        {
            return Result.Failure<UsuarioResponse>(actualizarResult.Error).ToHttpResult();
        }

        var response = new UsuarioResponse(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.NombreUsuario,
            usuario.Contrasena,
            usuario.Rol,
            usuario.Estado);

        return Result.Success(response).ToHttpResult();
    }
}