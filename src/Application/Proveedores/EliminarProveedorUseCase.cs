using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Proveedores;

public sealed class EliminarProveedorUseCase
{
    private readonly IProveedorRepository _proveedores;

    public EliminarProveedorUseCase(IProveedorRepository proveedores)
    {
        _proveedores = proveedores;
    }

    public async Task<Result> EjecutarAsync(int id, CancellationToken cancellationToken = default)
    {
        var proveedor = await _proveedores.ObtenerPorIdAsync(id, cancellationToken);
        if (proveedor is null)
        {
            return Result.Failure(ProveedorErrores.NoEncontrado(id));
        }

        var tienePedidos = await _proveedores.TienePedidosAsociadosAsync(id, cancellationToken);
        if (tienePedidos)
        {
            return Result.Failure(ProveedorErrores.TienePedidosAsociados(id));
        }

        await _proveedores.EliminarAsync(id, cancellationToken);
        return Result.Success();
    }
}