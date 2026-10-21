namespace DistriFresasLY.Api.Contracts.Pagos.Clientes;

public record PagoClienteResponse(
    int Id,
    int ClienteId,
    int VentaId,
    double Valor,
    string MedioPago,
    DateTime FechaPago,
    string Estado
);