using DistriFresasLY.Domain.Entities.Ventas;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato del repositorio de ventas.
/// Hereda el CRUD genérico de <see cref="IRepositorioGenerica{T}"/> y define
/// las verificaciones de integridad previas a eliminar una venta.
/// </summary>
public interface IVentaRepository : IRepositorioGenerica<Venta>
{
    /// <summary>
    /// Verifica si la venta tiene pagos de cliente asociados.
    /// </summary>
    Task<bool> TienePagosAsociadosAsync(
        int idVenta,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si la venta ya tiene un recibo generado.
    /// </summary>
    Task<bool> TieneReciboAsociadoAsync(
        int idVenta,
        CancellationToken cancellationToken = default);
}
