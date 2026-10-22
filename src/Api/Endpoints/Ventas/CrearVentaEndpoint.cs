using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Endpoints.Inventario;
using DistriFresasLY.Api.Extensions;
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
           .WithSummary("Registra una nueva venta");
    }

    private static IResult Manejador([FromBody] CrearVentaRequest request, [FromHeader(Name = "X-Es-Admin")] bool esAdministrador = false)
    {
        if (request.Detalles is null || !request.Detalles.Any())
        {
            return Result.Failure(Error.Validation(
                "Venta.Validacion",
                "La venta debe incluir al menos un producto en el detalle."))
                .ToHttpResult();
        }


        var stockDisponiblePorProducto = new Dictionary<int, int>();

        foreach (var d in request.Detalles)
        {
            var itemStock = InventarioDataStore.ObtenerPorProductoId(d.ProductoId);

            if (itemStock is null)
            {
                return Result.Failure(Error.NotFound(
                    "Inventario.NotFound",
                    $"No existe registro de inventario para el producto ID: {d.ProductoId}"))
                    .ToHttpResult();
            }

            stockDisponiblePorProducto[d.ProductoId] = itemStock.CantidadDisponible;
        }


        var detallesResult = request.Detalles
            .Select(d => DetalleVenta.Create(d.ProductoId, d.Cantidad, d.PrecioUnitario))
            .ToList();

        var detalleInvalido = detallesResult.FirstOrDefault(r => r.IsFailure);
        if (detalleInvalido is not null)
        {
            return Result.Failure<VentaResponse>(detalleInvalido.Error).ToHttpResult();
        }

        var detalles = detallesResult.Select(r => r.Value).ToList();

        var nuevoId = VentaDataStore.VentasDb.Count != 0
            ? VentaDataStore.VentasDb.Max(v => v.Id) + 1
            : 1;


        var ventaResult = Venta.Create(
            detalles,
            request.ClienteId ?? 0,
            stockDisponiblePorProducto,
            esAdministrador,
            nuevoId);

        if (ventaResult.IsFailure)
        {
            return Result.Failure<VentaResponse>(ventaResult.Error).ToHttpResult();
        }


        foreach (var d in request.Detalles)
        {
            InventarioDataStore.DisminuirStock(d.ProductoId, d.Cantidad);
        }

        var venta = ventaResult.Value;

        VentaDataStore.VentasDb.Add(venta);

        var response = new VentaResponse(
            venta.Id,
            venta.FechaVenta,
            venta.Total,
            venta.Estado,
            venta.ClienteId
        );

        return Result.Success(response).ToHttpCreatedAtResult($"/api/ventas/{venta.Id}");
    }
}