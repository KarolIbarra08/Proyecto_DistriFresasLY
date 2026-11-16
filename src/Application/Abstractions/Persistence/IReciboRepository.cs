using DistriFresasLY.Domain.Entities.Recibos;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato del repositorio de recibos.
/// Su CRUD completo lo aporta <see cref="IRepositorioGenerica{T}"/>.
/// </summary>
public interface IReciboRepository : IRepositorioGenerica<Recibo>
{
}
