using DistriFresasLY.Domain.Entities.Roles;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IRolRepository
{
    Task<Rol?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Rol?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Rol>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarAsync(Rol rol, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Rol rol, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}