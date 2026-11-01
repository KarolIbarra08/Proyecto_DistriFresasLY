using DistriFresasLY.Domain.Entities.Inventario;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IInventarioRepository
{
    Task<Inventario?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Inventario?> ObtenerPorProductoIdAsync(int productoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Inventario>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarAsync(Inventario inventario, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Inventario inventario, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}
