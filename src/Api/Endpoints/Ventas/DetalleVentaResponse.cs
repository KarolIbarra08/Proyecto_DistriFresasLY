namespace DistriFresasLY.Api.Contracts.Ventas;

public record DetalleVentaResponse(
    int ProductoId,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal,
    decimal VentaTotal
);