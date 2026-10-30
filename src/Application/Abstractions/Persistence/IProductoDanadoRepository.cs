using DistriFresasLY.Domain.Entities.ProductosDanados;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IProductoDanadoRepository
{
    Task<ProductoDanado?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductoDanado>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarAsync(ProductoDanado productoDanado, CancellationToken cancellationToken = default);
    Task ActualizarAsync(ProductoDanado productoDanado, CancellationToken cancellationToken = default);
}