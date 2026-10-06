using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

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

        var pedidoActual = PedidoDataStore.PedidosDb[index];

    
        List<DetallePedidoResponse> detallesActualizados;

        if (request.Detalles is not null)
        {
       
            PedidoDataStore.DetallesPedidoDb.RemoveAll(d => d.PedidoId == id);

            detallesActualizados = new List<DetallePedidoResponse>();

            foreach (var detalleReq in request.Detalles)
            {
                var nuevoDetalleId = PedidoDataStore.DetallesPedidoDb.Count != 0 
                    ? PedidoDataStore.DetallesPedidoDb.Max(d => d.Id) + 1 
                    : 1;

                double subtotal = detalleReq.Cantidad * detalleReq.ValorUnitario;

                var nuevoDetalle = new DetallePedidoResponse(
                    Id: nuevoDetalleId,
                    PedidoId: id,
                    Descripcion: detalleReq.Descripcion?.Trim() ?? string.Empty,
                    Cantidad: detalleReq.Cantidad,
                    ValorUnitario: detalleReq.ValorUnitario,
                    Subtotal: subtotal
                );

                PedidoDataStore.DetallesPedidoDb.Add(nuevoDetalle);
                detallesActualizados.Add(nuevoDetalle);
            }
        }
        else
        {
       
            detallesActualizados = PedidoDataStore.DetallesPedidoDb
                .Where(d => d.PedidoId == id)
                .ToList();
        }

   
        double nuevoTotal = request.Total ?? (detallesActualizados.Count > 0 
            ? detallesActualizados.Sum(d => d.Subtotal) 
            : pedidoActual.Total);

        var pedidoActualizado = pedidoActual with
        {
            Estado = !string.IsNullOrWhiteSpace(request.Estado) ? request.Estado.Trim() : pedidoActual.Estado,
            Total = nuevoTotal,
            Detalles = detallesActualizados
        };

        PedidoDataStore.PedidosDb[index] = pedidoActualizado;

        Result<PedidoResponse> successResult = Result.Success(pedidoActualizado);
        return successResult.ToHttpResult();
    }
}