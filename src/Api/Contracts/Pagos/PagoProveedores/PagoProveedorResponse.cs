namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public record PagoProveedorResponse(
    int Id,
    int ProveedorId,
    int PedidoId,
    double Valor,
    string MedioPago,
    DateTime FechaPago,
    string Estado
);
