using DistriFresasLY.Domain.Entities.Cliente;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato del repositorio de clientes.
/// Hereda el CRUD genérico de <see cref="IRepositorioGenerica{T}"/> y define
/// las consultas propias de los clientes: búsqueda por documento y
/// verificación de pedidos asociados.
/// </summary>
public interface IClienteRepository : IRepositorioGenerica<Cliente>
{
    /// <summary>
    /// Verifica si ya existe un cliente con el documento indicado.
    /// Permite excluir un identificador al actualizar un cliente.
    /// </summary>
    Task<bool> ExistePorDocumentoAsync(
        string cedulaNit,
        int? idExcluir = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si el cliente tiene pedidos asociados.
    /// </summary>
    Task<bool> TienePedidosAsociadosAsync(
        int idCliente,
        CancellationToken cancellationToken = default);
}
