namespace DistriFresasLY.Api.Contracts.Ventas;

public record VentaResponse(
    int Id,
    DateTime FechaVenta,
    double Total,
    string Estado,
    int? ClienteId
);