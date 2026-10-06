namespace DistriFresasLY.Api.Contracts.Pedidos;

public record ActualizarPedidoRequest(
    string? Estado,
    double? Total,
    List<CrearDetallePedidoRequest>? Detalles
);