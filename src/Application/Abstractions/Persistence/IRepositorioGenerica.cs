using DistriFresasLY.Domain.Common;
namespace DistriFresasLY.Application.Abstractions.Persistence;
public interface IRepositorioGenerica<T> where T : Entity
{
    Task<T?> ObtenerPorIdAsync(int id,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarAsync(T entidad,CancellationToken cancellationToken = default);
    Task ActualizarAsync(T entidad,CancellationToken cancellationToken = default);
    Task EliminarAsync( int id,CancellationToken cancellationToken = default);
}
