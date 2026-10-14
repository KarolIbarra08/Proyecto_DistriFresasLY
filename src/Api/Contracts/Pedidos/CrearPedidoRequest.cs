namespace DistriFresasLY.Api.Contracts.Pedidos;

public record CrearPedidoRequest(
    int ProveedorId,
    DateTime? FechaPedido,
    string? Estado,
    decimal? Total,
    List<CrearDetallePedidoRequest> Detalles
);
