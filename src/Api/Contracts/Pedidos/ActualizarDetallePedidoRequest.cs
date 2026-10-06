namespace DistriFresasLY.Api.Contracts.Pedidos;

public record ActualizarDetallePedidoRequest(
    string? Descripcion,
    int? Cantidad,
    double? ValorUnitario
);