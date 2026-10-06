namespace DistriFresasLY.Api.Contracts.Pagos.Clientes;

public record CrearPagoClienteRequest(
    int ClienteId,
    double Valor,
    string MedioPago,
    DateTime? FechaInicial,
    DateTime? FechaFinal,
    string Estado
);