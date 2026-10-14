namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public record ActualizarPagoProveedorRequest(
    int? ProveedorId,
    int? PedidoId,
    decimal? Valor,
    string? MedioPago,
    DateTime? FechaPago,
    string? Estado
);
