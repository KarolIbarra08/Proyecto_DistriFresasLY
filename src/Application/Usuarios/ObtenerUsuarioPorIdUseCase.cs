using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Usuarios;

public sealed class ObtenerUsuarioPorIdUseCase
{
    private readonly IUsuarioRepository _usuarios;

    public ObtenerUsuarioPorIdUseCase(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<Result<UsuarioDetalle>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(id, cancellationToken);
        if (usuario is null)
        {
            return Result.Failure<UsuarioDetalle>(UsuarioErrores.NoEncontrado(id));
        }

        return Result.Success(new UsuarioDetalle(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.NombreUsuario,
            usuario.Contrasena,
            usuario.Rol,
            usuario.Estado));
    }
}