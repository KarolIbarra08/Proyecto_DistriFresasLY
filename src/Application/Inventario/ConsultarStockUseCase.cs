using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Inventarios;

public sealed class ConsultarStockUseCase
{
    private readonly IInventarioRepository _inventarios;

    public ConsultarStockUseCase(IInventarioRepository inventarios) => _inventarios = inventarios;

    public async Task<Result<InventarioDetalleDto>> EjecutarAsync(
        int productoId,
        CancellationToken cancellationToken = default)
    {
        var inventario = await _inventarios.ObtenerPorProductoIdAsync(productoId, cancellationToken);
        if (inventario is null)
        {
            return Result.Failure<InventarioDetalleDto>(InventarioErrores.NoEncontradoPorProducto(productoId));
        }

        return Result.Success(new InventarioDetalleDto(
            inventario.Id,
            inventario.ProductoId,
            inventario.CantidadDisponible,
            inventario.FechaActualizacion));
    }
}