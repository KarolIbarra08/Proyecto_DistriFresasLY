using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Application.Recepciones;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Recepciones;
using InventarioEntity = DistriFresasLY.Domain.Entities.Inventario.Inventario;

namespace DistriFresasLY.Application.DetalleRecepciones;

public sealed record RegistrarDetalleRecepcionCommand(
    int IdRecepcion,
    int IdProducto,
    int Cantidad,
    double PrecioUnitario,
    DateTime? FechaIngreso);

public sealed class RegistrarDetalleRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;
    private readonly IProductoRepository _productos;
    private readonly IInventarioRepository _inventarios;

    public RegistrarDetalleRecepcionUseCase(
        IRecepcionRepository recepciones,
        IProductoRepository productos,
        IInventarioRepository inventarios)
    {
        _recepciones = recepciones;
        _productos = productos;
        _inventarios = inventarios;
    }

    public async Task<Result<DetalleRecepcionItemDto>> EjecutarAsync(
        RegistrarDetalleRecepcionCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (comando.IdRecepcion <= 0 || comando.IdProducto <= 0)
        {
            return Result.Failure<DetalleRecepcionItemDto>(DetalleRecepcionErrores.IdsInvalidos);
        }

        if (comando.Cantidad <= 0)
        {
            return Result.Failure<DetalleRecepcionItemDto>(DetalleRecepcionErrores.CantidadInvalida);
        }

        if (comando.PrecioUnitario <= 0)
        {
            return Result.Failure<DetalleRecepcionItemDto>(DetalleRecepcionErrores.PrecioInvalido);
        }

        var recepcion = await _recepciones.ObtenerPorIdAsync(comando.IdRecepcion, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<DetalleRecepcionItemDto>(
                DetalleRecepcionErrores.RecepcionNoEncontrada(comando.IdRecepcion));
        }

        var producto = await _productos.ObtenerPorIdAsync(comando.IdProducto, cancellationToken);
        if (producto is null)
        {
            return Result.Failure<DetalleRecepcionItemDto>(
                DetalleRecepcionErrores.ProductoNoEncontrado(comando.IdProducto));
        }

        var fechaReal = comando.FechaIngreso is { Year: > 1970 }
            ? comando.FechaIngreso.Value
            : ColombiaTime.Ahora;

        var detalleResult = DetalleRecepcion.Create(
            idProducto: comando.IdProducto,
            cantidad: comando.Cantidad,
            precioUnitario: comando.PrecioUnitario,
            idRecepcion: comando.IdRecepcion,
            fechaIngreso: fechaReal);

        if (detalleResult.IsFailure)
        {
            return Result.Failure<DetalleRecepcionItemDto>(detalleResult.Error);
        }

        var agregarResult = recepcion.AgregarDetalle(detalleResult.Value);
        if (agregarResult.IsFailure)
        {
            return Result.Failure<DetalleRecepcionItemDto>(agregarResult.Error);
        }

        await _recepciones.ActualizarAsync(recepcion, cancellationToken);

        var inventario = await _inventarios.ObtenerPorProductoIdAsync(comando.IdProducto, cancellationToken);

        if (inventario is null)
        {
            var nuevoInventario = InventarioEntity.Create(comando.IdProducto, comando.Cantidad);
            if (nuevoInventario.IsFailure)
            {
                return Result.Failure<DetalleRecepcionItemDto>(nuevoInventario.Error);
            }

            await _inventarios.AgregarAsync(nuevoInventario.Value, cancellationToken);
        }
        else
        {
            var aumentarResult = inventario.Aumentar(comando.Cantidad);
            if (aumentarResult.IsFailure)
            {
                return Result.Failure<DetalleRecepcionItemDto>(aumentarResult.Error);
            }

            await _inventarios.ActualizarAsync(inventario, cancellationToken);
        }

        var guardada = await _recepciones.ObtenerPorIdAsync(comando.IdRecepcion, cancellationToken);
        var detalle = guardada!.Detalles.LastOrDefault(d => d.IdProducto == comando.IdProducto);

        if (detalle is null)
        {
            return Result.Failure<DetalleRecepcionItemDto>(DetalleRecepcionErrores.ErrorCreacion);
        }

        return Result.Success(new DetalleRecepcionItemDto(
            detalle.Id,
            detalle.IdRecepcion,
            detalle.IdProducto,
            (int)detalle.Cantidad,
            detalle.PrecioUnitario,
            detalle.Subtotal,
            detalle.FechaIngreso));
    }
}