namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public record ActualizarPagoProveedorRequest(
    int? ProveedorId,
    decimal? Valor,
    string? MedioPago,
    DateTime? FechaInicial,
    DateTime? FechaFinal,
    string? Estado
);