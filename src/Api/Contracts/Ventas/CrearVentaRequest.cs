namespace DistriFresasLY.Api.Contracts.Ventas;

public record DetalleVentaRequest(
    int ProductoId,
    int Cantidad,
    decimal PrecioUnitario
);

public record CrearVentaRequest(
    int? ClienteId,
    List<DetalleVentaRequest> Detalles
);