using DistriFresasLY.Domain.Entities.Inventario;
namespace DistriFresasLY.Application.Abstractions.Persistence;
public interface IInventarioRepository : IRepositorioGenerica<Inventario>
{
    Task<Inventario?> ObtenerPorProductoIdAsync(int productoId,CancellationToken cancellationToken = default);
}
