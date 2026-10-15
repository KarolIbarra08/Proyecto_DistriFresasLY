namespace DistriFresasLY.Api.Contracts.Pedidos;

public record CrearPedidoRequest(
    int ProveedorId,
    DateTime? FechaPedido,
    string? Estado,
    double? Total,
    List<CrearDetallePedidoRequest> Detalles
);
