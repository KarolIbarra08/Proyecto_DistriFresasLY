namespace DistriFresasLY.Api.Contracts.Pedidos;

public record ActualizarPedidoRequest(
    int? ProveedorId,
    string? Estado,
    decimal? Total,
    List<CrearDetallePedidoRequest>? Detalles
);
