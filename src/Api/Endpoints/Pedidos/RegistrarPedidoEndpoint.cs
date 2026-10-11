using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Pedidos;

namespace DistriFresasLY.Api.Endpoints.Pedidos;

public class RegistrarPedidoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/pedidos", Manejador)
           .WithName("RegistrarPedido")
           .WithTags("Pedidos")
           .WithSummary("Registra un nuevo pedido con sus detalles");
    }

    private static IResult Manejador(CrearPedidoRequest request)
    {
        if (request.Detalles is null || !request.Detalles.Any())
        {
            Result<PedidoResponse> validationResult = Error.Validation(
                "Pedido.Validacion",
                "El pedido debe incluir al menos un producto en el detalle.");

            return validationResult.ToHttpResult();
        }

        var nuevoPedidoId = PedidoDataStore.PedidosDb.Count != 0
            ? PedidoDataStore.PedidosDb.Max(p => p.Id) + 1
            : 1;

        var detallesResult = request.Detalles
            .Select(detalleReq => DetallePedido.Create(
                pedidoId: nuevoPedidoId,
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

        var fechaPedido = request.FechaPedido ?? DateTime.UtcNow;
        var estado = string.IsNullOrWhiteSpace(request.Estado) ? "Pendiente" : request.Estado.Trim();

        var pedidoResult = Pedido.Create(detalles, fechaPedido, estado, nuevoPedidoId);
        if (pedidoResult.IsFailure)
        {
            return Result.Failure<PedidoResponse>(pedidoResult.Error).ToHttpResult();
        }

        var pedido = pedidoResult.Value;
        PedidoDataStore.PedidosDb.Add(pedido);

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

        Result<PedidoResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/pedidos/{pedido.Id}");
    }
}