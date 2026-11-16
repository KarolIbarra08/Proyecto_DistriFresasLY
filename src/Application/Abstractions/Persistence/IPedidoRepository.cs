using DistriFresasLY.Domain.Entities.Pedidos;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato del repositorio de pedidos a proveedores.
/// Su CRUD completo lo aporta <see cref="IRepositorioGenerica{T}"/>.
/// </summary>
public interface IPedidoRepository : IRepositorioGenerica<Pedido>
{
}
