using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Api.Endpoints.Pagos.Proveedores;

public class RegistrarPagoProveedorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/pagos/proveedores", Manejador)
           .WithName("RegistrarPagoProveedor")
           .WithTags("Pagos - Proveedores")
           .WithSummary("Registra un nuevo pago a proveedor");
    }

    private static async Task<IResult> Manejador(
        CrearPagoProveedorRequest request,
        IPedidoRepository pedidoRepository,
        IPagoProveedorRepository pagoRepository,
        CancellationToken cancellationToken)
    {
        var pedido = await pedidoRepository.ObtenerPorIdAsync(request.PedidoId, cancellationToken);
        if (pedido is null)
        {
            return Result.Failure<PagoProveedorResponse>(
                Error.NotFound("Pedido.NotFound", $"No existe el pedido con ID {request.PedidoId}"))
                .ToHttpResult();
        }

        if (pedido.ProveedorId != request.ProveedorId)
        {
            return Result.Failure<PagoProveedorResponse>(
                Error.Validation("PagoProveedor.ProveedorNoCoincide", "El proveedor del pago no coincide con el proveedor del pedido."))
                .ToHttpResult();
        }

        var pagos = await pagoRepository.ObtenerTodosAsync(cancellationToken);

        var pagadoAnteriormente = pagos
            .Where(p => p.PedidoId == request.PedidoId && p.Estado != "Anulado")
            .Sum(p => p.Valor);

        var saldoPendiente = pedido.Total - pagadoAnteriormente;

        if (saldoPendiente <= 0)
        {
            return Result.Failure<PagoProveedorResponse>(
                Error.Validation("PagoProveedor.PedidoSaldado", "Este pedido ya se encuentra totalmente pagado."))
                .ToHttpResult();
        }

        if (request.Valor > saldoPendiente)
        {
            return Result.Failure<PagoProveedorResponse>(
                Error.Validation("PagoProveedor.MontoExcedido",
                    $"El pago (${request.Valor}) supera el saldo pendiente del pedido (${saldoPendiente})."))
                .ToHttpResult();
        }

        var nuevoTotalPagado = pagadoAnteriormente + request.Valor;
        var estadoCalculado = nuevoTotalPagado >= pedido.Total ? "Completado" : "Abono Parcial";
        var estadoFinal = string.IsNullOrWhiteSpace(request.Estado) ? estadoCalculado : request.Estado.Trim();

        var pagoResult = PagoProveedor.Create(
            proveedorId: request.ProveedorId,
            pedidoId: request.PedidoId,
            valor: request.Valor,
            medioPago: request.MedioPago,
            fechaPago: request.FechaPago,
            estado: estadoFinal);

        if (pagoResult.IsFailure)
        {
            return Result.Failure<PagoProveedorResponse>(pagoResult.Error).ToHttpResult();
        }

        var nuevoId = await pagoRepository.AgregarAsync(pagoResult.Value, cancellationToken);
        var pago = await pagoRepository.ObtenerPorIdAsync(nuevoId, cancellationToken);

        if (pago is null)
        {
            return Result.Failure<PagoProveedorResponse>(
                Error.Failure("PagoProveedor.ErrorCreacion", "No se pudo recuperar el pago creado."))
                .ToHttpResult();
        }

        var response = new PagoProveedorResponse(
            pago.Id,
            pago.ProveedorId,
            pago.PedidoId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaPago,
            pago.Estado);

        return Result.Success(response).ToHttpCreatedAtResult($"/api/pagos/proveedores/{pago.Id}");
    }
}
