using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Endpoints.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

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

    private static IResult Manejador([FromBody] CrearVentaRequest request)
    {

        if (request.Detalles is null || !request.Detalles.Any())
        {
            return Result.Failure(Error.Validation(
                "Venta.Validacion",
                "La venta debe incluir al menos un producto en el detalle."))
                .ToHttpResult();
        }

 
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

            if (!itemStock.PuedeDisminuir(d.Cantidad))
            {
                return Result.Failure(Error.Validation(
                    "Inventario.StockInsuficiente",
                    $"Stock insuficiente para el producto {d.ProductoId}. Disponible: {itemStock.CantidadDisponible}, Requerido: {d.Cantidad}"))
                    .ToHttpResult();
            }

       
            itemStock.Disminuir(d.Cantidad);
        }

    
        var detallesModel = request.Detalles.Select(d => new DetalleVentaModel(
            d.ProductoId,
            d.Cantidad,
            d.PrecioUnitario,
            d.PrecioUnitario * d.Cantidad
        )).ToList();

        var nuevoId = VentaDataStore.VentasDb.Count != 0 
            ? VentaDataStore.VentasDb.Max(v => v.Id) + 1 
            : 1;

        var nuevaVenta = new VentaModel(
            nuevoId,
            DateTime.Now,
            detallesModel.Sum(x => x.Subtotal),
            "Pendiente",
            request.ClienteId,
            detallesModel
        );

    
        VentaDataStore.VentasDb.Add(nuevaVenta);

     
        var response = new VentaResponse(
            nuevaVenta.Id,
            nuevaVenta.FechaVenta,
            nuevaVenta.Total,
            nuevaVenta.Estado,
            nuevaVenta.ClienteId
        );

        return Result.Success(response).ToHttpCreatedAtResult($"/api/ventas/{nuevaVenta.Id}");
    }
}