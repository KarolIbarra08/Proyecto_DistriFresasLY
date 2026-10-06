namespace DistriFresasLY.Api.Contracts.Pedidos;

public record DetallePedidoResponse(
    int Id,
    int PedidoId,
    string Descripcion,
    int Cantidad,
    double ValorUnitario,
    double Subtotal
);