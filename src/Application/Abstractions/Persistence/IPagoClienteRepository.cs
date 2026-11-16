using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato del repositorio de pagos de clientes.
/// Su CRUD completo lo aporta <see cref="IRepositorioGenerica{T}"/>.
/// </summary>
public interface IPagoClienteRepository : IRepositorioGenerica<PagoCliente>
{
}
