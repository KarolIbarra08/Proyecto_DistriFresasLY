namespace DistriFresasLY.Api.Contracts.Pedidos;

public record ActualizarPedidoRequest(
    string? Estado,
    decimal? Total,
    List<CrearDetallePedidoRequest>? Detalles
);