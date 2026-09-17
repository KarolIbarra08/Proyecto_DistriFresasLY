namespace src.DistriFresasLY.Api.Contracts.Ventas;

public record DetalleVentaRequest(
    int ProductoId,
    int Cantidad,
    decimal PrecioUnitario
);