namespace DistriFresasLY.Application.Pagos;

public sealed record PagoClienteDetalleDto(
    int Id,
    int ClienteId,
    int VentaId,
    double Valor,
    string MedioPago,
    DateTime FechaPago,
    string Estado);
