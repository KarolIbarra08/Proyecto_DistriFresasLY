
using DistriFresasLY.Domain.Entities.Cliente;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato del repositorio de clientes.
/// Define las operaciones de consulta, registro, actualización
/// y eliminación de clientes en el sistema.
/// </summary>
public interface IClienteRepository
{
    /// <summary>
    /// Obtiene un cliente mediante su identificador.
    /// </summary>
    Task<Cliente?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los clientes registrados.
    /// </summary>
    Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Registra un nuevo cliente y devuelve su identificador.
    /// </summary>
    Task<int> AgregarAsync(
        Cliente cliente,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza la información de un cliente existente.
    /// </summary>
    Task ActualizarAsync(
        Cliente cliente,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un cliente mediante su identificador.
    /// </summary>
    Task EliminarAsync(
        int id,
        CancellationToken cancellationToken = default);
}
