using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Ventas;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public class CrearVentaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/ventas", Manejador)
           .WithName("CrearVenta")
           .WithTags("Ventas")
           .WithSummary("Registra una nueva venta y descuenta el inventario");
    }

    private static async Task<IResult> Manejador(
        CrearVentaRequest request,
        IVentaRepository ventaRepository,
        IClienteRepository clienteRepository,
        IInventarioRepository inventarioRepository,
        CancellationToken cancellationToken,
        [FromHeader(Name = "X-Es-Admin")] bool esAdministrador = false)
    {
        if (request.ClienteId is null || request.ClienteId.Value <= 0)
        {
            return Result.Failure<VentaResponse>(Error.Validation(
                "Venta.ClienteRequerido",
                "El cliente es obligatorio para registrar una venta."))
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

        if (request.Detalles is null || request.Detalles.Count == 0)
        {
            return Result.Failure<VentaResponse>(Error.Validation(
                "Venta.SinDetalles",
                "La venta debe contener al menos un producto en el detalle."))
                .ToHttpResult();
        }

        var detalles = new List<DetalleVenta>();
        var stockPorProducto = new Dictionary<int, int>();

        foreach (var detReq in request.Detalles)
        {
            var inventario = await inventarioRepository.ObtenerPorProductoIdAsync(detReq.ProductoId, cancellationToken);
            if (inventario is null)
            {
                return Result.Failure<VentaResponse>(Error.NotFound(
                    "Inventario.NotFound",
                    $"No existe registro de inventario para el producto ID: {detReq.ProductoId}"))
                    .ToHttpResult();
            }

            stockPorProducto[detReq.ProductoId] = inventario.CantidadDisponible;

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

        var ventaResult = Venta.Create(
            detalles: detalles,
            clienteId: request.ClienteId.Value,
            stockDisponiblePorProducto: stockPorProducto,
            esAdministrador: esAdministrador);

        if (ventaResult.IsFailure)
        {
            return Result.Failure<VentaResponse>(ventaResult.Error).ToHttpResult();
        }

        var nuevoId = await ventaRepository.AgregarAsync(ventaResult.Value, cancellationToken);

        foreach (var detReq in request.Detalles)
        {
            var inventario = await inventarioRepository.ObtenerPorProductoIdAsync(detReq.ProductoId, cancellationToken);
            if (inventario is null) continue;

            var disminuirResult = inventario.Disminuir(detReq.Cantidad);
            if (disminuirResult.IsFailure)
            {
                await ventaRepository.EliminarAsync(nuevoId, cancellationToken);
                return Result.Failure<VentaResponse>(disminuirResult.Error).ToHttpResult();
            }

            await inventarioRepository.ActualizarAsync(inventario, cancellationToken);
        }

        var ventaCreada = await ventaRepository.ObtenerPorIdAsync(nuevoId, cancellationToken);

        if (ventaCreada is null)
        {
            return Result.Failure<VentaResponse>(Error.Failure(
                "Venta.ErrorCreacion",
                "No se pudo recuperar la venta creada."))
                .ToHttpResult();
        }

        var response = new VentaResponse(
            ventaCreada.Id,
            ventaCreada.FechaVenta,
            ventaCreada.Total,
            ventaCreada.Estado.Value,
            ventaCreada.ClienteId == 0 ? null : ventaCreada.ClienteId);

        return Result.Success(response).ToHttpCreatedAtResult($"/api/ventas/{ventaCreada.Id}");
    }
}
