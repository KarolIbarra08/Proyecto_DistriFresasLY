namespace DistriFresasLY.Api.Contracts.Pedidos;

public record CrearPedidoRequest(
    DateTime? FechaPedido,
    string Estado,
    decimal Total,
    List<CrearDetallePedidoRequest> Detalles
);