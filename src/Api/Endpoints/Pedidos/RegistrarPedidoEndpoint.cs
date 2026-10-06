using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

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
        if (request.Total <= 0)
        {
            Result<PedidoResponse> validationResult = Error.Validation(
                "Pedido.Validacion",
                "El total del pedido debe ser mayor a cero.");

            return validationResult.ToHttpResult();
        }

        var nuevoPedidoId = PedidoDataStore.PedidosDb.Count != 0 
            ? PedidoDataStore.PedidosDb.Max(p => p.Id) + 1 
            : 1;

        var detallesGuardados = new List<DetallePedidoResponse>();

        if (request.Detalles is not null)
        {
            foreach (var detalleReq in request.Detalles)
            {
                var nuevoDetalleId = PedidoDataStore.DetallesPedidoDb.Count != 0 
                    ? PedidoDataStore.DetallesPedidoDb.Max(d => d.Id) + 1 
                    : 1;

                double subtotal = detalleReq.Cantidad * detalleReq.ValorUnitario;

                var nuevoDetalle = new DetallePedidoResponse(
                    Id: nuevoDetalleId,
                    PedidoId: nuevoPedidoId,
                    Descripcion: detalleReq.Descripcion?.Trim() ?? string.Empty,
                    Cantidad: detalleReq.Cantidad,
                    ValorUnitario: detalleReq.ValorUnitario,
                    Subtotal: subtotal
                );

                PedidoDataStore.DetallesPedidoDb.Add(nuevoDetalle);
                detallesGuardados.Add(nuevoDetalle);
            }
        }

        var nuevoPedido = new PedidoResponse(
            Id: nuevoPedidoId,
            FechaPedido: request.FechaPedido ?? DateTime.Now,
            Estado: string.IsNullOrWhiteSpace(request.Estado) ? "Pendiente" : request.Estado.Trim(),
            Total: request.Total,
            Detalles: detallesGuardados
        );

        PedidoDataStore.PedidosDb.Add(nuevoPedido);

        Result<PedidoResponse> createdResult = Result.Success(nuevoPedido);
        return createdResult.ToHttpCreatedAtResult($"/api/pedidos/{nuevoPedido.Id}");
    }
}