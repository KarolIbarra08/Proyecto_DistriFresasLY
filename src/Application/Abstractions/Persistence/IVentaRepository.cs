using DistriFresasLY.Domain.Entities.Ventas;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IVentaRepository
{
    Task<Venta?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Venta>> ObtenerTodosAsync(CancellationToken cancellationToken = default);

    Task<int> AgregarAsync(Venta venta, CancellationToken cancellationToken = default);

    Task ActualizarAsync(Venta venta, CancellationToken cancellationToken = default);

    Task EliminarAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> TienePagosAsociadosAsync(int idVenta, CancellationToken cancellationToken = default);

    Task<bool> TieneReciboAsociadoAsync(int idVenta, CancellationToken cancellationToken = default);
}
