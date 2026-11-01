using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IPagoClienteRepository
{
    Task<PagoCliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PagoCliente>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarAsync(PagoCliente pagoCliente, CancellationToken cancellationToken = default);
    Task ActualizarAsync(PagoCliente pagoCliente, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}
