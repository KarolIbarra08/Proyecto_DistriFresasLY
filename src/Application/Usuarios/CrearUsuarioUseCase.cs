using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Roles;
using DistriFresasLY.Domain.Entities.Usuarios;

namespace DistriFresasLY.Application.Usuarios;

public sealed record CrearUsuarioCommand(
    string Nombre,
    string Apellido,
    string Usuario,
    string Contrasena,
    string Rol,
    bool? Estado);

public sealed class CrearUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRolRepository _roles;

    public CrearUsuarioUseCase(IUsuarioRepository usuarios, IRolRepository roles)
    {
        _usuarios = usuarios;
        _roles = roles;
    }

    public async Task<Result<UsuarioDetalle>> EjecutarAsync(
        CrearUsuarioCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(comando.Usuario))
        {
            var usuarioExistente = await _usuarios.ObtenerPorNombreUsuarioAsync(comando.Usuario, cancellationToken);
            if (usuarioExistente is not null)
            {
                return Result.Failure<UsuarioDetalle>(UsuarioErrores.Duplicado(comando.Usuario));
            }
        }

        Rol? rolExistente = int.TryParse(comando.Rol, out var idRol)
            ? await _roles.ObtenerPorIdAsync(idRol, cancellationToken)
            : await _roles.ObtenerPorNombreAsync(comando.Rol ?? string.Empty, cancellationToken);

        if (rolExistente is null)
        {
            return Result.Failure<UsuarioDetalle>(UsuarioErrores.RolNoEncontrado(comando.Rol ?? string.Empty));
        }

        var usuarioResult = Usuario.Create(
            comando.Nombre,
            comando.Apellido,
            comando.Usuario,
            comando.Contrasena,
            rolExistente.Nombre,
            comando.Estado ?? true);

        if (usuarioResult.IsFailure)
        {
            return Result.Failure<UsuarioDetalle>(usuarioResult.Error);
        }

        var usuario = usuarioResult.Value;
        var idUsuario = await _usuarios.AgregarAsync(usuario, cancellationToken);

        return Result.Success(new UsuarioDetalle(
            idUsuario,
            usuario.Nombre,
            usuario.Apellido,
            usuario.NombreUsuario,
            usuario.Contrasena,
            rolExistente.Nombre,
            usuario.Estado));
    }
}