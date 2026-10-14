namespace DistriFresasLY.Api.Contracts.Pagos.Clientes;

public record PagoClienteResponse(
    int Id,
    int ClienteId,
    int VentaId,
    decimal Valor,
    string MedioPago,
    DateTime FechaPago,
    string Estado
);