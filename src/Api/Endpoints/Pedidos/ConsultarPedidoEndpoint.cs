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

        var detalles = PedidoDataStore.DetallesPedidoDb
            .Where(d => d.PedidoId == id)
            .ToList();

        var pedidoConDetalles = pedido with { Detalles = detalles };

        Result<PedidoResponse> successResult = Result.Success(pedidoConDetalles);
        return successResult.ToHttpResult();
    }
}