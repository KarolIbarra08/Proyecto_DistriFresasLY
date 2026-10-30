using DistriFresasLY.Domain.Entities.Proveedores;

namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IProveedorRepository
{
    Task<Proveedor?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Proveedor?> ObtenerPorCedulaNitAsync(string cedulaNit, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Proveedor>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<int> AgregarAsync(Proveedor proveedor, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Proveedor proveedor, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}