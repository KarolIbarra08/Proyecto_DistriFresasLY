namespace DistriFresasLY.Api.Contracts.Ventas;

public record CrearVentaRequest(
    int? ClienteId,
    List<DetalleVentaRequest> Detalles
);