using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        CrearPedidoRequest request,
        IProveedorRepository proveedorRepository,
        CancellationToken cancellationToken)
    {
        if (request.ProveedorId <= 0)
        {
            Result<PedidoResponse> validationResult = Error.Validation(
                "Pedido.ProveedorInvalido",
                "El ID del proveedor es obligatorio.");

            return validationResult.ToHttpResult();
        }

        var proveedor = await proveedorRepository.ObtenerPorIdAsync(request.ProveedorId, cancellationToken);
        if (proveedor is null)
        {
            Result<PedidoResponse> notFoundResult = Error.NotFound(
                "Pedido.ProveedorNoEncontrado",
                $"No se encontró un proveedor con ID {request.ProveedorId}.");

            return notFoundResult.ToHttpResult();
        }

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

        var maxDetalleId = PedidoDataStore.PedidosDb
            .SelectMany(p => p.Detalles)
            .Select(d => d.Id)
            .DefaultIfEmpty(0)
            .Max();

        var siguienteDetalleId = maxDetalleId + 1;

        var detallesResult = request.Detalles
            .Select((detalleReq, index) => DetallePedido.Create(
                pedidoId: nuevoPedidoId,
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

        var fechaPedido = request.FechaPedido ?? ColombiaTime.Ahora;
        var estado = string.IsNullOrWhiteSpace(request.Estado) ? "Pendiente" : request.Estado.Trim();

        var pedidoResult = Pedido.Create(request.ProveedorId, detalles, fechaPedido, estado, nuevoPedidoId);
        if (pedidoResult.IsFailure)
        {
            return Result.Failure<PedidoResponse>(pedidoResult.Error).ToHttpResult();
        }

        var pedido = pedidoResult.Value;
        PedidoDataStore.PedidosDb.Add(pedido);

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

        Result<PedidoResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/pedidos/{pedido.Id}");
    }
}