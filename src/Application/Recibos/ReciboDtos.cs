namespace DistriFresasLY.Application.Recibos;

public sealed record ReciboDetalleDto(
    int Id,
    DateTime FechaRecibo,
    int NumeroRecibo,
    double Total,
    int VentaId);
