namespace DistriFresasLY.Api.Contracts.Ventas;

public record VentaResponse(
    int Id,
    DateTime FechaVenta,
    decimal Total,
    string Estado,
    int? ClienteId
);