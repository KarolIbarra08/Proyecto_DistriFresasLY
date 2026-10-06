using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pedidos;

public class EliminarPedidoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/pedidos/{id:int}", Manejador)
           .WithName("EliminarPedido")
           .WithTags("Pedidos")
           .WithSummary("Elimina un pedido y todos sus detalles asociados");
    }

    private static IResult Manejador(int id)
    {
        var pedido = PedidoDataStore.PedidosDb.FirstOrDefault(p => p.Id == id);

        if (pedido is null)
        {
            Result<bool> notFoundResult = Error.NotFound(
                "Pedido.NotFound",
                $"No se encontro un pedido registrado con el ID '{id}'.");

            return notFoundResult.ToHttpResult();
        }

      
        PedidoDataStore.PedidosDb.Remove(pedido);
        PedidoDataStore.DetallesPedidoDb.RemoveAll(d => d.PedidoId == id);

        Result<bool> successResult = Result.Success(true);
        return successResult.ToHttpResult();
    }
}