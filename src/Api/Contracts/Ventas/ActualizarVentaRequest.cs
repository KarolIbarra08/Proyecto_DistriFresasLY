namespace DistriFresasLY.Api.Contracts.Ventas;

public record ActualizarVentaRequest(
    int? ClienteId,
    string? Estado,
    List<DetalleVentaRequest>? Detalles
);