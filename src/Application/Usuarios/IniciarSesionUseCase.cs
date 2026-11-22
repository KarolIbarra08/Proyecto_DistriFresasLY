using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Usuarios;

public sealed record IniciarSesionCommand(
    string Usuario,
    string Contrasena);

public sealed class IniciarSesionUseCase
{
    private readonly IUsuarioRepository _usuarios;

    public IniciarSesionUseCase(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<Result<UsuarioDetalle>> EjecutarAsync(
        IniciarSesionCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(comando.Usuario) || string.IsNullOrWhiteSpace(comando.Contrasena))
        {
            return Result.Failure<UsuarioDetalle>(UsuarioErrores.CredencialesObligatorias);
        }

        var usuario = await _usuarios.ObtenerPorNombreUsuarioAsync(comando.Usuario, cancellationToken);

        if (usuario is null || !usuario.VerificarCredenciales(comando.Usuario, comando.Contrasena))
        {
            return Result.Failure<UsuarioDetalle>(UsuarioErrores.CredencialesInvalidas);
        }

        if (!usuario.Estado)
        {
            return Result.Failure<UsuarioDetalle>(UsuarioErrores.Inactivo);
        }

        return Result.Success(new UsuarioDetalle(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.NombreUsuario,
            "*****",
            usuario.Rol,
            usuario.Estado));
    }
}