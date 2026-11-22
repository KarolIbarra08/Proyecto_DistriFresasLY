using InventarioEntity = DistriFresasLY.Domain.Entities.Inventario.Inventario;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IInventarioRepository : IRepositorioGenerica<InventarioEntity>
{
    Task<InventarioEntity?> ObtenerPorProductoIdAsync(int productoId, CancellationToken cancellationToken = default);
}