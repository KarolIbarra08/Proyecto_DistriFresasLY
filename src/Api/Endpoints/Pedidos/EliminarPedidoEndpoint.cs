using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        int id,
        IPedidoRepository pedidoRepository,
        IPagoProveedorRepository pagoRepository,
        CancellationToken cancellationToken)
    {
        var pedido = await pedidoRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (pedido is null)
        {
            return Result.Failure<bool>(Error.NotFound(
                "Pedido.NotFound",
                $"No se encontro un pedido registrado con el ID '{id}'."))
                .ToHttpResult();
        }

        var pagos = await pagoRepository.ObtenerTodosAsync(cancellationToken);
        if (pagos.Any(p => p.PedidoId == id))
        {
            return Result.Failure<bool>(Error.Conflict(
                "Pedido.PagosAsociados",
                "No se puede eliminar un pedido con pagos asociados."))
                .ToHttpResult();
        }

        await pedidoRepository.EliminarAsync(id, cancellationToken);

        return Result.Success(true).ToHttpResult();
    }
}
