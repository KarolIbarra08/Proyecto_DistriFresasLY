namespace DistriFresasLY.Api.Contracts.Pagos.Clientes;

public record CrearPagoClienteRequest(
    int ClienteId,
    decimal Valor,
    string MedioPago,
    DateTime? FechaInicial,
    DateTime? FechaFinal,
    string Estado
);