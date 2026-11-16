using DistriFresasLY.Domain.Entities.Proveedores;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato del repositorio de proveedores.
/// Hereda el CRUD genérico de <see cref="IRepositorioGenerica{T}"/> y define
/// la búsqueda por cédula o NIT.
/// </summary>
public interface IProveedorRepository : IRepositorioGenerica<Proveedor>
{
    /// <summary>
    /// Obtiene un proveedor mediante su cédula o NIT.
    /// </summary>
    Task<Proveedor?> ObtenerPorCedulaNitAsync(
        string cedulaNit,
        CancellationToken cancellationToken = default);
}
