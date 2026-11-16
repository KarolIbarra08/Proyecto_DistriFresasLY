using DistriFresasLY.Domain.Entities.Roles;

namespace DistriFresasLY.Application.Abstractions.Persistence;
public interface IRolRepository : IRepositorioGenerica<Rol>
{
    Task<Rol?> ObtenerPorNombreAsync( string nombre,CancellationToken cancellationToken = default);
}
