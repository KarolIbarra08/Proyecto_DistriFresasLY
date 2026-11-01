using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IPagoProveedorRepository
{
    Task<PagoProveedor?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PagoProveedor>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarAsync(PagoProveedor pagoProveedor, CancellationToken cancellationToken = default);
    Task ActualizarAsync(PagoProveedor pagoProveedor, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}
