using DistriFresasLY.Domain.Entities.Recepciones;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato del repositorio de recepciones.
/// Su CRUD completo lo aporta <see cref="IRepositorioGenerica{T}"/>.
/// </summary>
public interface IRecepcionRepository : IRepositorioGenerica<Recepcion>
{
}
