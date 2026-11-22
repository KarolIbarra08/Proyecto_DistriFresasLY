using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Roles;

namespace DistriFresasLY.Application.Usuarios;

public sealed record ActualizarUsuarioCommand(
    int Id,
    string? Nombre,
    string? Apellido,
    string? Usuario,
    string? Contrasena,
    string? Rol,
    bool? Estado);

public sealed class ActualizarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRolRepository _roles;

    public ActualizarUsuarioUseCase(IUsuarioRepository usuarios, IRolRepository roles)
    {
        _usuarios = usuarios;
        _roles = roles;
    }

    public async Task<Result<UsuarioDetalle>> EjecutarAsync(
        ActualizarUsuarioCommand comando,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (usuario is null)
        {
            return Result.Failure<UsuarioDetalle>(UsuarioErrores.NoEncontrado(comando.Id));
        }

        var nuevoNombre = !string.IsNullOrWhiteSpace(comando.Nombre) ? comando.Nombre : usuario.Nombre;
        var nuevoApellido = !string.IsNullOrWhiteSpace(comando.Apellido) ? comando.Apellido : usuario.Apellido;
        var nuevoNombreUsuario = !string.IsNullOrWhiteSpace(comando.Usuario) ? comando.Usuario : usuario.NombreUsuario;
        var nuevaContrasena = !string.IsNullOrWhiteSpace(comando.Contrasena) ? comando.Contrasena : usuario.Contrasena;
        var nuevoRol = !string.IsNullOrWhiteSpace(comando.Rol) ? comando.Rol : usuario.Rol;
        var nuevoEstado = comando.Estado ?? usuario.Estado;

        if (!nuevoNombreUsuario.Equals(usuario.NombreUsuario, StringComparison.OrdinalIgnoreCase))
        {
            var usuarioConMismoNombre = await _usuarios.ObtenerPorNombreUsuarioAsync(nuevoNombreUsuario, cancellationToken);
            if (usuarioConMismoNombre is not null && usuarioConMismoNombre.Id != comando.Id)
            {
                return Result.Failure<UsuarioDetalle>(UsuarioErrores.Duplicado(nuevoNombreUsuario));
            }
        }

        Rol? rolExistente = int.TryParse(nuevoRol, out var idRol)
            ? await _roles.ObtenerPorIdAsync(idRol, cancellationToken)
            : await _roles.ObtenerPorNombreAsync(nuevoRol, cancellationToken);

        if (rolExistente is null)
        {
            return Result.Failure<UsuarioDetalle>(UsuarioErrores.RolNoEncontrado(nuevoRol));
        }

        var actualizarResult = usuario.ActualizarDatos(
            nuevoNombre,
            nuevoApellido,
            nuevoNombreUsuario,
            nuevaContrasena,
            rolExistente.Nombre,
            nuevoEstado);

        if (actualizarResult.IsFailure)
        {
            return Result.Failure<UsuarioDetalle>(actualizarResult.Error);
        }

        await _usuarios.ActualizarAsync(usuario, cancellationToken);

        return Result.Success(new UsuarioDetalle(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.NombreUsuario,
            usuario.Contrasena,
            rolExistente.Nombre,
            usuario.Estado));
    }
}