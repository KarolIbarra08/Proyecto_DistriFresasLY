namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public record CrearPagoProveedorRequest(
    int ProveedorId,
    decimal Valor,
    string MedioPago,
    DateTime? FechaInicial,
    DateTime? FechaFinal,
    string Estado
);