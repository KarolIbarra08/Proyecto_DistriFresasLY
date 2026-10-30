using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Roles;

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

    private static async Task<IResult> Manejador(
        int id,
        ActualizarUsuarioRequest request,
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (usuario is null)
        {
            return Result.Failure<UsuarioResponse>(
                Error.NotFound(
                    "Usuario.NotFound",
                    $"No se encontró un usuario con el Id: {id}"))
                .ToHttpResult();
        }

        var nuevoNombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre : usuario.Nombre;
        var nuevoApellido = !string.IsNullOrWhiteSpace(request.Apellido) ? request.Apellido : usuario.Apellido;
        var nuevoNombreUsuario = !string.IsNullOrWhiteSpace(request.Usuario) ? request.Usuario : usuario.NombreUsuario;
        var nuevaContrasena = !string.IsNullOrWhiteSpace(request.Contrasena) ? request.Contrasena : usuario.Contrasena;
        var nuevoRol = !string.IsNullOrWhiteSpace(request.Rol) ? request.Rol : usuario.Rol;

        if (!nuevoNombreUsuario.Equals(usuario.NombreUsuario, StringComparison.OrdinalIgnoreCase))
        {
            var usuarioConMismoNombre = await usuarioRepository.ObtenerPorNombreUsuarioAsync(nuevoNombreUsuario, cancellationToken);
            if (usuarioConMismoNombre is not null && usuarioConMismoNombre.Id != id)
            {
                return Result.Failure<UsuarioResponse>(
                    Error.Conflict(
                        "Usuario.Duplicado",
                        $"El nombre de usuario '{nuevoNombreUsuario}' ya está en uso por otro usuario."))
                    .ToHttpResult();
            }
        }


        Rol? rolExistente = null;
        if (int.TryParse(nuevoRol, out int idRol))
        {
            rolExistente = await rolRepository.ObtenerPorIdAsync(idRol, cancellationToken);
        }
        else
        {
            rolExistente = await rolRepository.ObtenerPorNombreAsync(nuevoRol, cancellationToken);
        }

        if (rolExistente is null)
        {
            return Result.Failure<UsuarioResponse>(
                Error.NotFound(
                    "Usuario.RolNoEncontrado",
                    $"El rol '{nuevoRol}' especificado no existe en el sistema."))
                .ToHttpResult();
        }


        var actualizarResult = usuario.ActualizarDatos(
            nuevoNombre, 
            nuevoApellido, 
            nuevoNombreUsuario, 
            nuevaContrasena, 
            rolExistente.Id.ToString());

        if (actualizarResult.IsFailure)
        {
            return Result.Failure<UsuarioResponse>(actualizarResult.Error).ToHttpResult();
        }

        await usuarioRepository.ActualizarAsync(usuario, cancellationToken);

        var response = new UsuarioResponse(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.NombreUsuario,
            usuario.Contrasena,
            rolExistente.Nombre,
            usuario.Estado);

        return Result.Success(response).ToHttpResult();
    }
}