using DistriFresasLY.Domain.Entities.Inventario;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato del repositorio de inventario.
/// Hereda el CRUD genérico de <see cref="IRepositorioGenerica{T}"/> y define
/// la consulta por producto, necesaria para validar stock en ventas y recepciones.
/// </summary>
public interface IInventarioRepository : IRepositorioGenerica<Inventario>
{
    /// <summary>
    /// Obtiene el registro de inventario asociado a un producto.
    /// </summary>
    Task<Inventario?> ObtenerPorProductoIdAsync(
        int productoId,
        CancellationToken cancellationToken = default);
}
