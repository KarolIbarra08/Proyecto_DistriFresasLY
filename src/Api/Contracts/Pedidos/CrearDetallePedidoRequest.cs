namespace DistriFresasLY.Api.Contracts.Pedidos;

public record CrearDetallePedidoRequest(
    int PedidoId,
    string Descripcion,
    int Cantidad,
    double ValorUnitario
);