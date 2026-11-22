using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Proveedores;

public sealed class ObtenerProveedoresUseCase
{
    private readonly IProveedorRepository _proveedores;

    public ObtenerProveedoresUseCase(IProveedorRepository proveedores)
    {
        _proveedores = proveedores;
    }

    public async Task<Result<IReadOnlyList<ProveedorDetalle>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var proveedores = await _proveedores.ObtenerTodosAsync(cancellationToken);

        var detalles = proveedores.Select(p => new ProveedorDetalle(
            p.Id,
            p.Nombre,
            p.Apellido,
            p.Documento.Value,
            p.Telefono.Valor,
            p.Direccion,
            p.NombreEmpresa)).ToList();

        return Result.Success<IReadOnlyList<ProveedorDetalle>>(detalles);
    }
}