namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public record CrearPagoProveedorRequest(
    int ProveedorId,
    int PedidoId,
    double Valor,
    string MedioPago,
    DateTime? FechaPago,
    string Estado
);
