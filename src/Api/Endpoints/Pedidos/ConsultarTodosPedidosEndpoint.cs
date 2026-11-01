using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        IPedidoRepository pedidoRepository,
        CancellationToken cancellationToken)
    {
        var pedidos = await pedidoRepository.ObtenerTodosAsync(cancellationToken);

        var response = pedidos.Select(pedido => new PedidoResponse(
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
                d.Subtotal)).ToList())).ToList();

        return Result.Success(response).ToHttpResult();
    }
}
