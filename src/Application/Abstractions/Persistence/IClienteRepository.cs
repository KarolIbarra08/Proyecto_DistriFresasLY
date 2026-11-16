using DistriFresasLY.Domain.Entities.Cliente;
namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IClienteRepository : IRepositorioGenerica<Cliente>
{
    Task<bool> ExistePorDocumentoAsync(string cedulaNit,int? idExcluir = null, CancellationToken cancellationToken = default);
    Task<bool> TienePedidosAsociadosAsync(int idCliente, CancellationToken cancellationToken = default);
}
