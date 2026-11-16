using DistriFresasLY.Domain.Entities.Roles;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato del repositorio de roles.
/// Hereda el CRUD genérico de <see cref="IRepositorioGenerica{T}"/> y define
/// la búsqueda por nombre, utilizada al iniciar sesión.
/// </summary>
public interface IRolRepository : IRepositorioGenerica<Rol>
{
    /// <summary>
    /// Obtiene un rol mediante su nombre.
    /// </summary>
    Task<Rol?> ObtenerPorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default);
}
