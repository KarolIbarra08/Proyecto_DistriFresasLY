namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public record PagoProveedorResponse(
    int Id,
    int ProveedorId,
    decimal Valor,
    string MedioPago,
    DateTime FechaInicial,
    DateTime FechaFinal,
    string Estado
);