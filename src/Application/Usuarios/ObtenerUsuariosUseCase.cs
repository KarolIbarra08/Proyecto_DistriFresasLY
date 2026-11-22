using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Usuarios;

public sealed class ObtenerUsuariosUseCase
{
    private readonly IUsuarioRepository _usuarios;

    public ObtenerUsuariosUseCase(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<Result<IReadOnlyList<UsuarioDetalle>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var usuarios = await _usuarios.ObtenerTodosAsync(cancellationToken);

        var detalles = usuarios.Select(u => new UsuarioDetalle(
            u.Id,
            u.Nombre,
            u.Apellido,
            u.NombreUsuario,
            u.Contrasena,
            u.Rol,
            u.Estado)).ToList();

        return Result.Success<IReadOnlyList<UsuarioDetalle>>(detalles);
    }
}