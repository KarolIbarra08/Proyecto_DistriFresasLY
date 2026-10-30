using DistriFresasLY.Domain.Entities.Pedidos;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IPedidoRepository
{
    Task<Pedido?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pedido>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarAsync(Pedido pedido, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Pedido pedido, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}