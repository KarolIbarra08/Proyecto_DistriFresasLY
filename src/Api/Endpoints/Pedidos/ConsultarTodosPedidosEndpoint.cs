using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pedidos;

public class ConsultarTodosPedidosEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/pedidos", Manejador)
           .WithName("ConsultarTodosPedidos")
           .WithTags("Pedidos")
           .WithSummary("Consulta todos los pedidos con sus detalles");
    }

    private static IResult Manejador()
    {
        var listaPedidos = PedidoDataStore.PedidosDb.Select(pedido => new PedidoResponse(
            pedido.Id,
            pedido.ProveedorId,
            pedido.FechaPedido,
            pedido.Estado,
            pedido.Total,
            pedido.Detalles.Select(d => new DetallePedidoResponse(
                d.Id,
                d.PedidoId,
                d.Descripcion,
                d.Cantidad,
                d.ValorUnitario,
                d.Subtotal)).ToList()
        )).ToList();

        Result<List<PedidoResponse>> successResult = Result.Success(listaPedidos);
        return successResult.ToHttpResult();
    }
}