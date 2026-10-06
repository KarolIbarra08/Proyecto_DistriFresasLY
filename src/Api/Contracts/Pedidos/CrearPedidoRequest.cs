namespace DistriFresasLY.Api.Contracts.Pedidos;

public record CrearPedidoRequest(
    DateTime? FechaPedido,
    string Estado,
    double Total,
    List<CrearDetallePedidoRequest> Detalles
);