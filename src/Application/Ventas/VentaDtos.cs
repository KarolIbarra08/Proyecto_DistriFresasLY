namespace DistriFresasLY.Application.Ventas;

public sealed record VentaDetalleDto(
    int Id,
    DateTime FechaVenta,
    double Total,
    string Estado,
    int? ClienteId);

public sealed record DetalleVentaDto(
    int ProductoId,
    int Cantidad,
    double PrecioUnitario,
    double Subtotal,
    double TotalVenta);
