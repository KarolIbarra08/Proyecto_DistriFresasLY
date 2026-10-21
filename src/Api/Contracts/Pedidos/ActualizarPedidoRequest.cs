namespace DistriFresasLY.Api.Contracts.Pedidos;

public record ActualizarPedidoRequest(
    int? ProveedorId,
    string? Estado,
    double? Total,
    List<CrearDetallePedidoRequest>? Detalles
);
