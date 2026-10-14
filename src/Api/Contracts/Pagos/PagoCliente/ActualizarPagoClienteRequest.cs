namespace DistriFresasLY.Api.Contracts.Pagos.Clientes;

public record ActualizarPagoClienteRequest(
    int? ClienteId,
    int? VentaId,
    decimal? Valor,
    string? MedioPago,
    DateTime? FechaPago,
    string? Estado
);