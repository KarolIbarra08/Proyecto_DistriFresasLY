namespace DistriFresasLY.Api.Contracts.Pagos.Clientes;

public record CrearPagoClienteRequest(
    int ClienteId,
    int VentaId,
    decimal Valor,
    string MedioPago,
    DateTime? FechaPago,
    string Estado
);