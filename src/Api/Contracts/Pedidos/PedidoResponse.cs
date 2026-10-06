namespace DistriFresasLY.Api.Contracts.Pedidos;

public record PedidoResponse(
    int Id,
    DateTime FechaPedido,
    string Estado,
    double Total,
    List<DetallePedidoResponse> Detalles
);