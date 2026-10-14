namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public record PagoProveedorResponse(
    int Id,
    int ProveedorId,
    int PedidoId,
    decimal Valor,
    string MedioPago,
    DateTime FechaPago,
    string Estado
);
