using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Proveedores;

public sealed class ObtenerProveedorPorIdUseCase
{
    private readonly IProveedorRepository _proveedores;

    public ObtenerProveedorPorIdUseCase(IProveedorRepository proveedores)
    {
        _proveedores = proveedores;
    }

    public async Task<Result<ProveedorDetalle>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var proveedor = await _proveedores.ObtenerPorIdAsync(id, cancellationToken);
        if (proveedor is null)
        {
            return Result.Failure<ProveedorDetalle>(ProveedorErrores.NoEncontrado(id));
        }

        return Result.Success(new ProveedorDetalle(
            proveedor.Id,
            proveedor.Nombre,
            proveedor.Apellido,
            proveedor.Documento.Value,
            proveedor.Telefono.Valor,
            proveedor.Direccion,
            proveedor.NombreEmpresa));
    }
}