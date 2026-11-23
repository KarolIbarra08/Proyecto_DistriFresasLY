using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Ventas;

namespace DistriFresasLY.Application.Ventas;

public sealed record ActualizarVentaCommand(
    int Id,
    int? ClienteId,
    string? Estado,
    IReadOnlyList<CrearDetalleVentaCommand>? Detalles);

public sealed class ActualizarVentaUseCase
{
    private readonly IVentaRepository _ventas;
    private readonly IClienteRepository _clientes;
    private readonly IInventarioRepository _inventarios;

    public ActualizarVentaUseCase(
        IVentaRepository ventas,
        IClienteRepository clientes,
        IInventarioRepository inventarios)
    {
        _ventas = ventas;
        _clientes = clientes;
        _inventarios = inventarios;
    }

    public async Task<Result<VentaDetalleDto>> EjecutarAsync(
        ActualizarVentaCommand comando,
        CancellationToken cancellationToken = default)
    {
        var venta = await _ventas.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (venta is null)
        {
            return Result.Failure<VentaDetalleDto>(VentaErrores.NoEncontrada(comando.Id));
        }

        if (comando.ClienteId.HasValue)
        {
            if (comando.ClienteId.Value <= 0)
            {
                return Result.Failure<VentaDetalleDto>(VentaErrores.ClienteRequerido);
            }

            var cliente = await _clientes.ObtenerPorIdAsync(comando.ClienteId.Value, cancellationToken);
            if (cliente is null)
            {
                return Result.Failure<VentaDetalleDto>(
                    VentaErrores.ClienteNoEncontrado(comando.ClienteId.Value));
            }

            var clienteResult = venta.AsignarCliente(comando.ClienteId.Value);
            if (clienteResult.IsFailure)
            {
                return Result.Failure<VentaDetalleDto>(clienteResult.Error);
            }
        }

        var cantidadesAnteriores = venta.Detalles
            .GroupBy(d => d.ProductoId)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad.Value));

        Dictionary<int, int>? cantidadesNuevas = null;

        if (comando.Detalles is not null)
        {
            var detalles = new List<DetalleVenta>();

            foreach (var detReq in comando.Detalles)
            {
                var detalleResult = DetalleVenta.Create(
                    productoId: detReq.ProductoId,
                    cantidad: detReq.Cantidad,
                    precioUnitario: detReq.PrecioUnitario);

                if (detalleResult.IsFailure)
                {
                    return Result.Failure<VentaDetalleDto>(detalleResult.Error);
                }

                detalles.Add(detalleResult.Value);
            }

            cantidadesNuevas = detalles
                .GroupBy(d => d.ProductoId)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad.Value));

            foreach (var producto in cantidadesNuevas)
            {
                var anterior = cantidadesAnteriores.GetValueOrDefault(producto.Key, 0);
                var incremento = producto.Value - anterior;

                if (incremento <= 0) continue;

                var inventario = await _inventarios.ObtenerPorProductoIdAsync(producto.Key, cancellationToken);
                if (inventario is null)
                {
                    return Result.Failure<VentaDetalleDto>(
                        VentaErrores.InventarioNoEncontrado(producto.Key));
                }

                if (inventario.CantidadDisponible < incremento)
                {
                    return Result.Failure<VentaDetalleDto>(
                        VentaErrores.StockInsuficiente(producto.Key, inventario.CantidadDisponible, incremento));
                }
            }

            var detallesResult = venta.ActualizarDetalles(detalles);
            if (detallesResult.IsFailure)
            {
                return Result.Failure<VentaDetalleDto>(detallesResult.Error);
            }
        }

        if (!string.IsNullOrWhiteSpace(comando.Estado))
        {
            var estadoResult = venta.ActualizarEstado(comando.Estado.Trim());
            if (estadoResult.IsFailure)
            {
                return Result.Failure<VentaDetalleDto>(estadoResult.Error);
            }
        }

        await _ventas.ActualizarAsync(venta, cancellationToken);

        if (cantidadesNuevas is not null)
        {
            var productos = cantidadesAnteriores.Keys.Union(cantidadesNuevas.Keys);

            foreach (var productoId in productos)
            {
                var anterior = cantidadesAnteriores.GetValueOrDefault(productoId, 0);
                var nuevo = cantidadesNuevas.GetValueOrDefault(productoId, 0);
                var diferencia = nuevo - anterior;

                if (diferencia == 0) continue;

                var inventario = await _inventarios.ObtenerPorProductoIdAsync(productoId, cancellationToken);
                if (inventario is null) continue;

                var movimiento = diferencia > 0
                    ? inventario.Disminuir(diferencia)
                    : inventario.Aumentar(-diferencia);

                if (movimiento.IsFailure)
                {
                    return Result.Failure<VentaDetalleDto>(movimiento.Error);
                }

                await _inventarios.ActualizarAsync(inventario, cancellationToken);
            }
        }

        return Result.Success(new VentaDetalleDto(
            venta.Id,
            venta.FechaVenta,
            venta.Total,
            venta.Estado.Value,
            venta.ClienteId == 0 ? null : venta.ClienteId));
    }
}
