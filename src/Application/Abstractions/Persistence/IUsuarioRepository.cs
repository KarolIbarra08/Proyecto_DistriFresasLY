using DistriFresasLY.Domain.Entities.Usuarios;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato del repositorio de usuarios.
/// Hereda el CRUD genérico de <see cref="IRepositorioGenerica{T}"/> y define
/// la búsqueda por nombre de usuario, utilizada al iniciar sesión.
/// </summary>
public interface IUsuarioRepository : IRepositorioGenerica<Usuario>
{
    /// <summary>
    /// Obtiene un usuario mediante su nombre de usuario.
    /// </summary>
    Task<Usuario?> ObtenerPorNombreUsuarioAsync(
        string nombreUsuario,
        CancellationToken cancellationToken = default);
}
