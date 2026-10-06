namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public record PagoProveedorResponse(
    int Id,
    int ProveedorId,
    double Valor,
    string MedioPago,
    DateTime FechaInicial,
    DateTime FechaFinal,
    string Estado
);