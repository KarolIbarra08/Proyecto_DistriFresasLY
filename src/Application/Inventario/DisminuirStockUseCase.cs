using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using InventarioEntity = DistriFresasLY.Domain.Entities.Inventario.Inventario;

namespace DistriFresasLY.Application.Inventarios;

public sealed class DisminuirStockUseCase
{
    private readonly IInventarioRepository _inventarios;

    public DisminuirStockUseCase(IInventarioRepository inventarios) => _inventarios = inventarios;

    public async Task<Result<InventarioDetalleDto>> EjecutarAsync(
        ActualizarStockCommand comando,
        CancellationToken cancellationToken = default)
    {
        InventarioEntity? inventario = await _inventarios.ObtenerPorProductoIdAsync(comando.ProductoId, cancellationToken);
        if (inventario is null)
        {
            return Result.Failure<InventarioDetalleDto>(InventarioErrores.NoEncontradoPorProducto(comando.ProductoId));
        }

        var disminuirResult = inventario.Disminuir(comando.Cantidad);
        if (disminuirResult.IsFailure)
        {
            return Result.Failure<InventarioDetalleDto>(disminuirResult.Error);
        }

        await _inventarios.ActualizarAsync(inventario, cancellationToken);

        return Result.Success(new InventarioDetalleDto(
            inventario.Id,
            inventario.ProductoId,
            inventario.CantidadDisponible,
            inventario.FechaActualizacion));
    }
}