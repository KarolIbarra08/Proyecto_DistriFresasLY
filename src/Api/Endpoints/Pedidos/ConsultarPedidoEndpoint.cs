using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pedidos;

public class ConsultarPedidoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/pedidos/{id:int}", Manejador)
           .WithName("ConsultarPedido")
           .WithTags("Pedidos")
           .WithSummary("Consulta un pedido por su ID con sus detalles");
    }

    private static IResult Manejador(int id)
    {
        var pedido = PedidoDataStore.PedidosDb.FirstOrDefault(p => p.Id == id);

        if (pedido is null)
        {
            Result<PedidoResponse> notFoundResult = Error.NotFound(
                "Pedido.NotFound",
                $"No se encontro un pedido registrado con el ID '{id}'.");

            return notFoundResult.ToHttpResult();
        }

        var response = new PedidoResponse(
            pedido.Id,
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
        );

        Result<PedidoResponse> successResult = Result.Success(response);
        return successResult.ToHttpResult();
    }
}