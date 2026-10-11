namespace DistriFresasLY.Api.Contracts.Pagos.Clientes;

public record ActualizarPagoClienteRequest(
    int? ClienteId,
    decimal? Valor,
    string? MedioPago,
    DateTime? FechaInicial,
    DateTime? FechaFinal,
    string? Estado
);