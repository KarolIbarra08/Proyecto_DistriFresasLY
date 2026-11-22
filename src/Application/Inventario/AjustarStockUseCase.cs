using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using InventarioEntity = DistriFresasLY.Domain.Entities.Inventario.Inventario;


namespace DistriFresasLY.Application.Inventarios;

public sealed record ActualizarStockCommand(
    int ProductoId,
    int Cantidad);

public sealed class AjustarStockUseCase
{
    private readonly IInventarioRepository _inventarios;
    private readonly IProductoRepository _productos;

    public AjustarStockUseCase(
        IInventarioRepository inventarios,
        IProductoRepository productos)
    {
        _inventarios = inventarios;
        _productos = productos;
    }

    public async Task<Result<InventarioDetalleDto>> EjecutarAsync(
        ActualizarStockCommand comando,
        CancellationToken cancellationToken = default)
    {
        var inventario = await _inventarios.ObtenerPorProductoIdAsync(comando.ProductoId, cancellationToken);
        if (inventario is null)
        {
            var productoExiste = await _productos.ObtenerPorIdAsync(comando.ProductoId, cancellationToken);
            if (productoExiste is null)
            {
                return Result.Failure<InventarioDetalleDto>(InventarioErrores.ProductoNoEncontrado(comando.ProductoId));
            }

            var nuevoInventarioResult = InventarioEntity.Create(comando.ProductoId, comando.Cantidad);
            if (nuevoInventarioResult.IsFailure)
            {
                return Result.Failure<InventarioDetalleDto>(nuevoInventarioResult.Error);
            }

            var nuevoId = await _inventarios.AgregarAsync(nuevoInventarioResult.Value, cancellationToken);
            var creado = await _inventarios.ObtenerPorIdAsync(nuevoId, cancellationToken);

            if (creado is null)
            {
                return Result.Failure<InventarioDetalleDto>(InventarioErrores.ErrorCreacion);
            }

            return Result.Success(new InventarioDetalleDto(
                creado.Id,
                creado.ProductoId,
                creado.CantidadDisponible,
                creado.FechaActualizacion));
        }

        var ajustarResult = inventario.Ajustar(comando.Cantidad);
        if (ajustarResult.IsFailure)
        {
            return Result.Failure<InventarioDetalleDto>(ajustarResult.Error);
        }

        await _inventarios.ActualizarAsync(inventario, cancellationToken);

        return Result.Success(new InventarioDetalleDto(
            inventario.Id,
            inventario.ProductoId,
            inventario.CantidadDisponible,
            inventario.FechaActualizacion));
    }
}