using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Inventarios;

public sealed class AumentarStockUseCase
{
    private readonly IInventarioRepository _inventarios;

    public AumentarStockUseCase(IInventarioRepository inventarios) => _inventarios = inventarios;

    public async Task<Result<InventarioDetalleDto>> EjecutarAsync(
        ActualizarStockCommand comando,
        CancellationToken cancellationToken = default)
    {
        var inventario = await _inventarios.ObtenerPorProductoIdAsync(comando.ProductoId, cancellationToken);
        if (inventario is null)
        {
            return Result.Failure<InventarioDetalleDto>(InventarioErrores.NoEncontradoPorProducto(comando.ProductoId));
        }

        var aumentarResult = inventario.Aumentar(comando.Cantidad);
        if (aumentarResult.IsFailure)
        {
            return Result.Failure<InventarioDetalleDto>(aumentarResult.Error);
        }

        await _inventarios.ActualizarAsync(inventario, cancellationToken);

        return Result.Success(new InventarioDetalleDto(
            inventario.Id,
            inventario.ProductoId,
            inventario.CantidadDisponible,
            inventario.FechaActualizacion));
    }
}