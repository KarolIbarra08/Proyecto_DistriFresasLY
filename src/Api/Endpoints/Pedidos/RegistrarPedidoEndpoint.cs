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
        IPedidoRepository pedidoRepository,
        CancellationToken cancellationToken)
    {
        if (request.ProveedorId <= 0)
        {
            return Result.Failure<PedidoResponse>(Error.Validation(
                "Pedido.ProveedorInvalido",
                "El ID del proveedor es obligatorio."))
                .ToHttpResult();
        }

        var proveedor = await proveedorRepository.ObtenerPorIdAsync(request.ProveedorId, cancellationToken);
        if (proveedor is null)
        {
            return Result.Failure<PedidoResponse>(Error.NotFound(
                "Pedido.ProveedorNoEncontrado",
                $"No se encontró un proveedor con ID {request.ProveedorId}."))
                .ToHttpResult();
        }

        if (request.Detalles is null || request.Detalles.Count == 0)
        {
            return Result.Failure<PedidoResponse>(Error.Validation(
                "Pedido.SinDetalles",
                "El pedido debe incluir al menos un producto en el detalle."))
                .ToHttpResult();
        }

        var detalles = new List<DetallePedido>();

        foreach (var detReq in request.Detalles)
        {
            var detalleResult = DetallePedido.Create(
                pedidoId: 0,
                descripcion: detReq.Descripcion,
                cantidad: detReq.Cantidad,
                valorUnitario: detReq.ValorUnitario);

            if (detalleResult.IsFailure)
            {
                return Result.Failure<PedidoResponse>(detalleResult.Error).ToHttpResult();
            }

            detalles.Add(detalleResult.Value);
        }

        var estado = string.IsNullOrWhiteSpace(request.Estado) ? "Pendiente" : request.Estado.Trim();
        var fechaPedido = request.FechaPedido ?? ColombiaTime.Ahora;

        var pedidoResult = Pedido.Create(
            proveedorId: request.ProveedorId,
            detalles: detalles,
            fechaPedido: fechaPedido,
            estado: estado);

        if (pedidoResult.IsFailure)
        {
            return Result.Failure<PedidoResponse>(pedidoResult.Error).ToHttpResult();
        }

        var nuevoId = await pedidoRepository.AgregarAsync(pedidoResult.Value, cancellationToken);
        var pedido = await pedidoRepository.ObtenerPorIdAsync(nuevoId, cancellationToken);

        if (pedido is null)
        {
            return Result.Failure<PedidoResponse>(Error.Failure(
                "Pedido.ErrorCreacion",
                "No se pudo recuperar el pedido creado."))
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

        return Result.Success(response).ToHttpCreatedAtResult($"/api/pedidos/{pedido.Id}");
    }
}
