using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Api.Endpoints.Pagos.Clientes;

public class RegistrarPagoClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/pagos/clientes", Manejador)
           .WithName("RegistrarPagoCliente")
           .WithTags("Pagos - Clientes")
           .WithSummary("Registra un abono o pago completo asociado a una venta");
    }

    private static async Task<IResult> Manejador(
        CrearPagoClienteRequest request,
        IVentaRepository ventaRepository,
        IPagoClienteRepository pagoRepository,
        CancellationToken cancellationToken)
    {
        var venta = await ventaRepository.ObtenerPorIdAsync(request.VentaId, cancellationToken);
        if (venta is null)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.NotFound("Venta.NotFound", $"No existe la venta con ID {request.VentaId}"))
                .ToHttpResult();
        }

        if (venta.ClienteId != request.ClienteId)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation("PagoCliente.ClienteNoCoincide", "El ClienteID no coincide con el cliente de la venta."))
                .ToHttpResult();
        }

        var pagos = await pagoRepository.ObtenerTodosAsync(cancellationToken);

        var pagadoAnteriormente = pagos
            .Where(p => p.VentaId == request.VentaId && p.Estado != "Anulado")
            .Sum(p => p.Valor);

        var saldoPendiente = venta.Total - pagadoAnteriormente;

        if (saldoPendiente <= 0)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation("PagoCliente.VentaSaldada", "Esta venta ya se encuentra totalmente pagada."))
                .ToHttpResult();
        }

        if (request.Valor > saldoPendiente)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation("PagoCliente.MontoExcedido",
                    $"El abono (${request.Valor}) supera el saldo pendiente de la venta (${saldoPendiente})."))
                .ToHttpResult();
        }

        var nuevoTotalPagado = pagadoAnteriormente + request.Valor;
        var estadoCalculado = nuevoTotalPagado >= venta.Total ? "Completado" : "Abono Parcial";
        var estadoFinal = string.IsNullOrWhiteSpace(request.Estado) ? estadoCalculado : request.Estado.Trim();

        var pagoResult = PagoCliente.Create(
            clienteId: request.ClienteId,
            ventaId: request.VentaId,
            valor: request.Valor,
            medioPago: request.MedioPago,
            fechaPago: request.FechaPago,
            estado: estadoFinal);

        if (pagoResult.IsFailure)
        {
            return Result.Failure<PagoClienteResponse>(pagoResult.Error).ToHttpResult();
        }

        var nuevoId = await pagoRepository.AgregarAsync(pagoResult.Value, cancellationToken);
        var pago = await pagoRepository.ObtenerPorIdAsync(nuevoId, cancellationToken);

        if (pago is null)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Failure("PagoCliente.ErrorCreacion", "No se pudo recuperar el pago creado."))
                .ToHttpResult();
        }

        var response = new PagoClienteResponse(
            pago.Id,
            pago.ClienteId,
            pago.VentaId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaPago,
            pago.Estado);

        return Result.Success(response).ToHttpCreatedAtResult($"/api/pagos/clientes/{pago.Id}");
    }
}
