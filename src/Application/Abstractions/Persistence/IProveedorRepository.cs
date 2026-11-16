using DistriFresasLY.Domain.Entities.Proveedores;
namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IProveedorRepository : IRepositorioGenerica<Proveedor>
{
    Task<Proveedor?> ObtenerPorCedulaNitAsync(string cedulaNit,CancellationToken cancellationToken = default);
}
