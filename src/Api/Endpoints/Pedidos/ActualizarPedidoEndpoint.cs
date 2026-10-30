using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Endpoints.Pagos.Proveedores;
using DistriFresasLY.Api.Endpoints.Pagos;
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
        IProveedorRepository proveedorRepository,
        CancellationToken cancellationToken)
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

        if (request.ProveedorId.HasValue)
        {
            if (request.ProveedorId.Value <= 0)
            {
                return Result.Failure<PedidoResponse>(Error.Validation("Pedido.ProveedorInvalido", "El ID del proveedor es obligatorio.")).ToHttpResult();
            }

            var proveedor = await proveedorRepository.ObtenerPorIdAsync(request.ProveedorId.Value, cancellationToken);
            if (proveedor is null)
            {
                return Result.Failure<PedidoResponse>(Error.NotFound("Pedido.ProveedorNoEncontrado", $"No se encontró un proveedor con ID {request.ProveedorId.Value}.")).ToHttpResult();
            }

            if (request.ProveedorId.Value != pedido.ProveedorId && PagoDataStore.PagosProveedoresDb.Any(p => p.PedidoId == id))
            {
                return Result.Failure<PedidoResponse>(Error.Conflict("Pedido.PagosAsociados", "No se puede cambiar el proveedor de un pedido con pagos asociados.")).ToHttpResult();
            }

            var proveedorResult = pedido.ActualizarProveedor(request.ProveedorId.Value);
            if (proveedorResult.IsFailure)
            {
                return Result.Failure<PedidoResponse>(proveedorResult.Error).ToHttpResult();
            }
        }

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
            var maxDetalleId = PedidoDataStore.PedidosDb
                .SelectMany(p => p.Detalles)
                .Select(d => d.Id)
                .DefaultIfEmpty(0)
                .Max();

            var siguienteDetalleId = maxDetalleId + 1;

            var detallesResult = request.Detalles
                .Select((detalleReq, index) => DetallePedido.Create(
                    pedidoId: id,
                    descripcion: detalleReq.Descripcion,
                    cantidad: detalleReq.Cantidad,
                    valorUnitario: detalleReq.ValorUnitario,
                    id: siguienteDetalleId + index))
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
                d.Subtotal)).ToList()
        );

        Result<PedidoResponse> successResult = Result.Success(response);
        return successResult.ToHttpResult();
    }
}