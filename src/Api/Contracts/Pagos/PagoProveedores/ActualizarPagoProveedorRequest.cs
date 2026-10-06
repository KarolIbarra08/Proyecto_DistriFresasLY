namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public record ActualizarPagoProveedorRequest(
    int? ProveedorId,
    double? Valor,
    string? MedioPago,
    DateTime? FechaInicial,
    DateTime? FechaFinal,
    string? Estado
);