using DistriFresasLY.Api.Contracts.Pedidos;

namespace DistriFresasLY.Api.Endpoints.Pedidos;

public static class PedidoDataStore
{
    public static readonly List<PedidoResponse> PedidosDb = [];
    public static readonly List<DetallePedidoResponse> DetallesPedidoDb = [];
}