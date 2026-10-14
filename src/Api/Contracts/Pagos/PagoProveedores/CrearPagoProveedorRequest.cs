namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public record CrearPagoProveedorRequest(
    int ProveedorId,
    int PedidoId,
    decimal Valor,
    string MedioPago,
    DateTime? FechaPago,
    string Estado
);
