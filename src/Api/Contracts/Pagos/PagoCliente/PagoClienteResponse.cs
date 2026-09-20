namespace DistriFresasLY.Api.Contracts.Pagos.Clientes;

public record PagoClienteResponse(
    int Id,
    int ClienteId,
    double Valor,
    string MedioPago,
    DateTime FechaInicial,
    DateTime FechaFinal,
    string Estado
);