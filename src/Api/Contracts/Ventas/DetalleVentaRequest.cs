namespace DistriFresasLY.Api.Contracts.Ventas;

public record DetalleVentaRequest(
    int ProductoId,
    int Cantidad,
    double PrecioUnitario
);
