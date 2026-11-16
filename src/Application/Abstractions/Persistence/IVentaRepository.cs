using DistriFresasLY.Domain.Entities.Ventas;

namespace DistriFresasLY.Application.Abstractions.Persistence;


public interface IVentaRepository : IRepositorioGenerica<Venta>
{
 
    Task<bool> TienePagosAsociadosAsync(int idVenta,CancellationToken cancellationToken = default);

    Task<bool> TieneReciboAsociadoAsync(int idVenta,CancellationToken cancellationToken = default);
}
