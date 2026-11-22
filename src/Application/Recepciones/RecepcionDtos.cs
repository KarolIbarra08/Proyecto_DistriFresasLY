namespace DistriFresasLY.Application.Recepciones;

public sealed record RecepcionDetalleDto(
    int Id,
    int? PedidoId,
    DateTime FechaRecepcion,
    double ValorPago,
    bool ProcesadaEnInventario,
    double TotalCalculado);

public sealed record DetalleRecepcionItemDto(
    int Id,
    int IdRecepcion,
    int IdProducto,
    int Cantidad,
    double PrecioUnitario,
    double Subtotal,
    DateTime FechaIngreso);