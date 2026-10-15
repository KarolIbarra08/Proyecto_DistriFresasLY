namespace DistriFresasLY.Api.Contracts.Ventas;

public record DetalleVentaResponse(
    int ProductoId,
    int Cantidad,
    double PrecioUnitario,
    double Subtotal,
    double TotalVenta
);