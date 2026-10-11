using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Pedidos;

namespace DistriFresasLY.Api.Endpoints.Pedidos;

public class ActualizarPedidoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/pedidos/{id:int}", Manejador)
           .WithName("ActualizarPedido")
           .WithTags("Pedidos")
           .WithSummary("Actualiza un pedido existente junto con sus detalles");
    }

    private static IResult Manejador(int id, ActualizarPedidoRequest request)
    {
        var index = PedidoDataStore.PedidosDb.FindIndex(p => p.Id == id);

        if (index == -1)
        {
            Result<PedidoResponse> notFoundResult = Error.NotFound(
                "Pedido.NotFound",
                $"No se encontro un pedido registrado con el ID '{id}'.");

            return notFoundResult.ToHttpResult();
        }

        var pedido = PedidoDataStore.PedidosDb[index];

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            var estadoResult = pedido.ActualizarEstado(request.Estado.Trim());
            if (estadoResult.IsFailure)
            {
                return Result.Failure<PedidoResponse>(estadoResult.Error).ToHttpResult();
            }
        }

        if (request.Detalles is not null)
        {
            var detallesResult = request.Detalles
                .Select(detalleReq => DetallePedido.Create(
                    pedidoId: id,
                    descripcion: detalleReq.Descripcion,
                    cantidad: detalleReq.Cantidad,
                    valorUnitario: detalleReq.ValorUnitario))
                .ToList();

            var detalleInvalido = detallesResult.FirstOrDefault(r => r.IsFailure);
            if (detalleInvalido is not null)
            {
                return Result.Failure<PedidoResponse>(detalleInvalido.Error).ToHttpResult();
            }

            var detalles = detallesResult.Select(r => r.Value).ToList();

            var detallesResultado = pedido.ActualizarDetalles(detalles);
            if (detallesResultado.IsFailure)
            {
                return Result.Failure<PedidoResponse>(detallesResultado.Error).ToHttpResult();
            }
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