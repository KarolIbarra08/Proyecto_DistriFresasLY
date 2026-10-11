namespace DistriFresasLY.Api.Contracts.Pedidos;

public record PedidoResponse(
    int Id,
    DateTime FechaPedido,
    string Estado,
    decimal Total,
    List<DetallePedidoResponse> Detalles
);