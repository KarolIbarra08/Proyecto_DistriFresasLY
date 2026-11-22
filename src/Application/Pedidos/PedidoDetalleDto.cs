namespace DistriFresasLY.Application.Pedidos;

public sealed record DetallePedidoItemDto(
    int Id,
    int PedidoId,
    string Descripcion,
    int Cantidad,
    double ValorUnitario,
    double Subtotal);

public sealed record PedidoDetalleDto(
    int Id,
    int ProveedorId,
    DateTime FechaPedido,
    string Estado,
    double Total,
    IReadOnlyList<DetallePedidoItemDto> Detalles);