using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        int id,
        IPedidoRepository pedidoRepository,
        CancellationToken cancellationToken)
    {
        var pedido = await pedidoRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (pedido is null)
        {
            return Result.Failure<PedidoResponse>(Error.NotFound(
                "Pedido.NotFound",
                $"No se encontro un pedido registrado con el ID '{id}'."))
                .ToHttpResult();
        }

        var response = new PedidoResponse(
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
                d.Subtotal)).ToList());

        return Result.Success(response).ToHttpResult();
    }
}
