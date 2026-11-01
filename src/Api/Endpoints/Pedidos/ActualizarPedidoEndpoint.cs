using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        int id,
        ActualizarPedidoRequest request,
        IPedidoRepository pedidoRepository,
        IProveedorRepository proveedorRepository,
        IPagoProveedorRepository pagoRepository,
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

        if (request.ProveedorId.HasValue)
        {
            if (request.ProveedorId.Value <= 0)
            {
                return Result.Failure<PedidoResponse>(Error.Validation(
                    "Pedido.ProveedorInvalido",
                    "El ID del proveedor es obligatorio."))
                    .ToHttpResult();
            }

            var proveedor = await proveedorRepository.ObtenerPorIdAsync(request.ProveedorId.Value, cancellationToken);
            if (proveedor is null)
            {
                return Result.Failure<PedidoResponse>(Error.NotFound(
                    "Pedido.ProveedorNoEncontrado",
                    $"No se encontró un proveedor con ID {request.ProveedorId.Value}."))
                    .ToHttpResult();
            }

            if (request.ProveedorId.Value != pedido.ProveedorId)
            {
                var pagos = await pagoRepository.ObtenerTodosAsync(cancellationToken);
                if (pagos.Any(p => p.PedidoId == id))
                {
                    return Result.Failure<PedidoResponse>(Error.Conflict(
                        "Pedido.PagosAsociados",
                        "No se puede cambiar el proveedor de un pedido con pagos asociados."))
                        .ToHttpResult();
                }
            }

            var proveedorResult = pedido.ActualizarProveedor(request.ProveedorId.Value);
            if (proveedorResult.IsFailure)
            {
                return Result.Failure<PedidoResponse>(proveedorResult.Error).ToHttpResult();
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            var estadoResult = pedido.ActualizarEstado(request.Estado);
            if (estadoResult.IsFailure)
            {
                return Result.Failure<PedidoResponse>(estadoResult.Error).ToHttpResult();
            }
        }

        if (request.Detalles is not null)
        {
            var detalles = new List<DetallePedido>();

            foreach (var detReq in request.Detalles)
            {
                var detalleResult = DetallePedido.Create(
                    pedidoId: id,
                    descripcion: detReq.Descripcion,
                    cantidad: detReq.Cantidad,
                    valorUnitario: detReq.ValorUnitario);

                if (detalleResult.IsFailure)
                {
                    return Result.Failure<PedidoResponse>(detalleResult.Error).ToHttpResult();
                }

                detalles.Add(detalleResult.Value);
            }

            var detallesResult = pedido.ActualizarDetalles(detalles);
            if (detallesResult.IsFailure)
            {
                return Result.Failure<PedidoResponse>(detallesResult.Error).ToHttpResult();
            }
        }

        await pedidoRepository.ActualizarAsync(pedido, cancellationToken);

        var actualizado = await pedidoRepository.ObtenerPorIdAsync(id, cancellationToken);

        var response = new PedidoResponse(
            actualizado!.Id,
            actualizado.ProveedorId,
            actualizado.FechaPedido,
            actualizado.Estado,
            actualizado.Total,
            actualizado.Detalles.Select(d => new DetallePedidoResponse(
                d.Id,
                d.PedidoId,
                d.Descripcion,
                d.Cantidad,
                d.ValorUnitario,
                d.Subtotal)).ToList());

        return Result.Success(response).ToHttpResult();
    }
}
