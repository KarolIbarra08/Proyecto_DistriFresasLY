namespace DistriFresasLY.Api.Contracts.Pagos.Clientes;

public record PagoClienteResponse(
    int Id,
    int ClienteId,
    decimal Valor,
    string MedioPago,
    DateTime FechaInicial,
    DateTime FechaFinal,
    string Estado
);