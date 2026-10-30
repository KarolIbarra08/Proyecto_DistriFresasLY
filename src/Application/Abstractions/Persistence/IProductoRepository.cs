using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IProductoRepository
{
    Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Producto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    
    // Operaciones de Insumos
    Task<IReadOnlyList<ProductoInsumo>> ObtenerTodosInsumosAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarInsumoAsync(ProductoInsumo insumo, CancellationToken cancellationToken = default);
    Task ActualizarInsumoAsync(ProductoInsumo insumo, CancellationToken cancellationToken = default);

    // Operaciones de Fresas
    Task<IReadOnlyList<ProductoFresa>> ObtenerTodasFresasAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarFresaAsync(ProductoFresa fresa, CancellationToken cancellationToken = default);
    Task ActualizarFresaAsync(ProductoFresa fresa, CancellationToken cancellationToken = default);
    Task<bool> ExisteCalibreAsync(int calibre, int? idExcluir = null, CancellationToken cancellationToken = default);

    // Operación general de eliminación
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}