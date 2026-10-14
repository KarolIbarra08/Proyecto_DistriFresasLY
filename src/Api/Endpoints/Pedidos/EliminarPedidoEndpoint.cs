using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Endpoints.Pagos;
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

        if (PagoDataStore.PagosProveedoresDb.Any(p => p.PedidoId == id))
        {
            return Result.Failure<bool>(Error.Conflict("Pedido.PagosAsociados", "No se puede eliminar un pedido con pagos asociados.")).ToHttpResult();
        }

        PedidoDataStore.PedidosDb.Remove(pedido);

        Result<bool> successResult = Result.Success(true);
        return successResult.ToHttpResult();
    }
}