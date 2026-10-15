namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public record ActualizarPagoProveedorRequest(
    int? ProveedorId,
    int? PedidoId,
    double? Valor,
    string? MedioPago,
    DateTime? FechaPago,
    string? Estado
);
