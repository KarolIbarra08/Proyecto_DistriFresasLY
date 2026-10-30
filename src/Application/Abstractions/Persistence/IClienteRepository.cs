using DistriFresasLY.Domain.Entities.Cliente;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IClienteRepository
{
    Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    
    Task<bool> ExistePorDocumentoAsync(string cedulaNit, int? idExcluir = null, CancellationToken cancellationToken = default);
    
    Task<bool> TienePedidosAsociadosAsync(int idCliente, CancellationToken cancellationToken = default);
    
    Task<int> AgregarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    
    Task ActualizarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}