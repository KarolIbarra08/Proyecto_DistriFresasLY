using DistriFresasLY.Domain.Entities.Recepciones;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IRecepcionRepository
{
    Task<Recepcion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Recepcion>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarAsync(Recepcion recepcion, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Recepcion recepcion, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}
