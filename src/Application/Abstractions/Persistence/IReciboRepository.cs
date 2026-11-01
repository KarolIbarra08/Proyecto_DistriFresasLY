using DistriFresasLY.Domain.Entities.Recibos;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IReciboRepository
{
    Task<Recibo?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Recibo>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarAsync(Recibo recibo, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Recibo recibo, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}
