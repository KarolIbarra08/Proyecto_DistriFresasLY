using DistriFresasLY.Api.Contracts.Ventas;
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

    private static IResult Manejador(CrearVentaRequest request)
    {
        if (request.Detalles is null || !request.Detalles.Any())
        {
            Result<VentaResponse> validationResult = Error.Validation(
                "Venta.Validacion",
                "La venta debe incluir al menos un producto en el detalle.");

            return validationResult.ToHttpResult();
        }

        
        var detallesModel = request.Detalles.Select(d => new DetalleVentaModel(
            d.ProductoId,
            d.Cantidad,
            d.PrecioUnitario,
            d.PrecioUnitario * d.Cantidad // En la API se asigna directamente
        )).ToList();

        var nuevoId = VentaDataStore.VentasDb.Count != 0 
            ? VentaDataStore.VentasDb.Max(v => v.Id) + 1 
            : 1;


        var nuevaVenta = new VentaModel(
            Id: nuevoId,
            FechaVenta: DateTime.Now,
            Total: detallesModel.Sum(x => x.Subtotal), 
            Estado: "Pendiente",
            ClienteId: request.ClienteId,
            Detalles: detallesModel 
        );

        VentaDataStore.VentasDb.Add(nuevaVenta);

        var response = new VentaResponse(
            nuevaVenta.Id,
            nuevaVenta.FechaVenta,
            nuevaVenta.Total,
            nuevaVenta.Estado,
            nuevaVenta.ClienteId
        );

        Result<VentaResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/ventas/{nuevaVenta.Id}");
    }
}