namespace DistriFresasLY.Application.Pagos;

public sealed record PagoProveedorDetalleDto(
    int Id,
    int ProveedorId,
    int PedidoId,
    double Valor,
    string MedioPago,
    DateTime FechaPago,
    string Estado);
