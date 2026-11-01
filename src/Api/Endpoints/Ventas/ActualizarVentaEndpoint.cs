using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Ventas;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public class ActualizarVentaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/ventas/{id:int}", Manejador)
           .WithName("ActualizarVenta")
           .WithTags("Ventas")
           .WithSummary("Actualiza una venta existente y ajusta el inventario");
    }

    private static async Task<IResult> Manejador(
        int id,
        ActualizarVentaRequest request,
        IVentaRepository ventaRepository,
        IClienteRepository clienteRepository,
        IInventarioRepository inventarioRepository,
        CancellationToken cancellationToken)
    {
        var venta = await ventaRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (venta is null)
        {
            return Result.Failure<VentaResponse>(
                Error.NotFound("Venta.NotFound", $"No se encontró la venta con ID {id}."))
                .ToHttpResult();
        }

        if (request.ClienteId.HasValue)
        {
            if (request.ClienteId.Value <= 0)
            {
                return Result.Failure<VentaResponse>(Error.Validation(
                    "Venta.ClienteRequerido",
                    "El ID del cliente debe ser mayor a 0."))
                    .ToHttpResult();
            }

            var cliente = await clienteRepository.ObtenerPorIdAsync(request.ClienteId.Value, cancellationToken);
            if (cliente is null)
            {
                return Result.Failure<VentaResponse>(Error.NotFound(
                    "Venta.ClienteNoEncontrado",
                    $"No se encontró un cliente con ID {request.ClienteId.Value}."))
                    .ToHttpResult();
            }

            var clienteResult = venta.AsignarCliente(request.ClienteId.Value);
            if (clienteResult.IsFailure)
            {
                return Result.Failure<VentaResponse>(clienteResult.Error).ToHttpResult();
            }
        }

        var cantidadesAnteriores = venta.Detalles
            .GroupBy(d => d.ProductoId)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad.Value));

        Dictionary<int, int>? cantidadesNuevas = null;

        if (request.Detalles is not null)
        {
            var detalles = new List<DetalleVenta>();

            foreach (var detReq in request.Detalles)
            {
                var detalleResult = DetalleVenta.Create(
                    productoId: detReq.ProductoId,
                    cantidad: detReq.Cantidad,
                    precioUnitario: detReq.PrecioUnitario);

                if (detalleResult.IsFailure)
                {
                    return Result.Failure<VentaResponse>(detalleResult.Error).ToHttpResult();
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

                var inventario = await inventarioRepository.ObtenerPorProductoIdAsync(producto.Key, cancellationToken);
                if (inventario is null)
                {
                    return Result.Failure<VentaResponse>(Error.NotFound(
                        "Inventario.NotFound",
                        $"No existe registro de inventario para el producto ID: {producto.Key}"))
                        .ToHttpResult();
                }

                if (inventario.CantidadDisponible < incremento)
                {
                    return Result.Failure<VentaResponse>(Error.Validation(
                        "Venta.StockInsuficiente",
                        $"No hay stock suficiente para el producto ID {producto.Key}. Disponible: {inventario.CantidadDisponible}, solicitado: {incremento}."))
                        .ToHttpResult();
                }
            }

            var detallesResult = venta.ActualizarDetalles(detalles);
            if (detallesResult.IsFailure)
            {
                return Result.Failure<VentaResponse>(detallesResult.Error).ToHttpResult();
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            var estadoResult = venta.ActualizarEstado(request.Estado.Trim());
            if (estadoResult.IsFailure)
            {
                return Result.Failure<VentaResponse>(estadoResult.Error).ToHttpResult();
            }
        }

        await ventaRepository.ActualizarAsync(venta, cancellationToken);

        if (cantidadesNuevas is not null)
        {
            var productos = cantidadesAnteriores.Keys.Union(cantidadesNuevas.Keys);

            foreach (var productoId in productos)
            {
                var anterior = cantidadesAnteriores.GetValueOrDefault(productoId, 0);
                var nuevo = cantidadesNuevas.GetValueOrDefault(productoId, 0);
                var diferencia = nuevo - anterior;

                if (diferencia == 0) continue;

                var inventario = await inventarioRepository.ObtenerPorProductoIdAsync(productoId, cancellationToken);
                if (inventario is null) continue;

                var movimiento = diferencia > 0
                    ? inventario.Disminuir(diferencia)
                    : inventario.Aumentar(-diferencia);

                if (movimiento.IsFailure)
                {
                    return Result.Failure<VentaResponse>(movimiento.Error).ToHttpResult();
                }

                await inventarioRepository.ActualizarAsync(inventario, cancellationToken);
            }
        }

        var response = new VentaResponse(
            venta.Id,
            venta.FechaVenta,
            venta.Total,
            venta.Estado.Value,
            venta.ClienteId == 0 ? null : venta.ClienteId);

        return Result.Success(response).ToHttpResult();
    }
}
