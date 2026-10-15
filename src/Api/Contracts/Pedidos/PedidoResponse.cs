namespace DistriFresasLY.Api.Contracts.Pedidos;

public record PedidoResponse(
    int Id,
    int ProveedorId,
    DateTime FechaPedido,
    string Estado,
    double Total,
    List<DetallePedidoResponse> Detalles
);
