
using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Api.Endpoints.Pagos.Clientes;

public class RegistrarPagoClienteEndpoint : IEndpoint
{
    
    private static readonly string[] EstadosProhibidosVenta =
    {
        "Cancelada",
        "Anulada"
    };

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
        
        if (request.ClienteId <= 0 || request.VentaId <= 0)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation(
                    "PagoCliente.IdsInvalidos",
                    "El ClienteId y VentaId deben ser obligatorios y mayores a 0."))
                .ToHttpResult();
        }

      
        if (request.Valor <= 0)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation(
                    "PagoCliente.ValorInvalido",
                    "El valor del pago debe ser mayor a 0."))
                .ToHttpResult();
        }

        
        var venta = await ventaRepository.ObtenerPorIdAsync(
            request.VentaId,
            cancellationToken);

        if (venta is null)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.NotFound(
                    "Venta.NotFound",
                    $"No existe la venta con ID {request.VentaId}."))
                .ToHttpResult();
        }

       
        if (venta.ClienteId != request.ClienteId)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation(
                    "PagoCliente.ClienteNoCoincide",
                    "El ClienteId no coincide con el cliente de la venta."))
                .ToHttpResult();
        }


        if (EstadosProhibidosVenta.Contains(venta.Estado))
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation(
                    "PagoCliente.VentaCerrada",
                    $"No se pueden registrar pagos porque la venta #{request.VentaId} se encuentra en estado '{venta.Estado}'."))
                .ToHttpResult();
        }

        var pagos = await pagoRepository.ObtenerTodosAsync(
            cancellationToken);

        var pagadoAnteriormente = pagos
            .Where(p =>
                p.VentaId == request.VentaId &&
                p.Estado != "Anulado" &&
                p.Estado != "Cancelado")
            .Sum(p => p.Valor);


        var saldoPendiente = venta.Total - pagadoAnteriormente;

        if (saldoPendiente <= 0)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation(
                    "PagoCliente.VentaSaldada",
                    "Esta venta ya se encuentra totalmente pagada."))
                .ToHttpResult();
        }


        if (request.Valor > saldoPendiente)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation(
                    "PagoCliente.MontoExcedido",
                    $"El abono (${request.Valor}) supera el saldo pendiente de la venta (${saldoPendiente})."))
                .ToHttpResult();
        }

        var nuevoTotalPagado = pagadoAnteriormente + request.Valor;

        var estadoFinal = nuevoTotalPagado >= venta.Total
            ? "Completado"
            : "Abono Parcial";

        var pagoResult = PagoCliente.Create(
            clienteId: request.ClienteId,
            ventaId: request.VentaId,
            valor: request.Valor,
            medioPago: request.MedioPago,
            fechaPago: request.FechaPago,
            estado: estadoFinal);

        if (pagoResult.IsFailure)
        {
            return Result.Failure<PagoClienteResponse>(
                pagoResult.Error)
                .ToHttpResult();
        }

        var nuevoId = await pagoRepository.AgregarAsync(
            pagoResult.Value,
            cancellationToken);

        
        if (nuevoTotalPagado >= venta.Total)
        {
            var actualizarVentaResult = venta.ActualizarEstado("Completada");

            if (actualizarVentaResult.IsFailure)
            {
                return Result.Failure<PagoClienteResponse>(
                    actualizarVentaResult.Error)
                    .ToHttpResult();
            }

            await ventaRepository.ActualizarAsync(
                venta,
                cancellationToken);
        }


        var pago = await pagoRepository.ObtenerPorIdAsync(
            nuevoId,
            cancellationToken);

        if (pago is null)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Failure(
                    "PagoCliente.ErrorCreacion",
                    "No se pudo recuperar el pago creado."))
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

        return Result.Success(response)
            .ToHttpCreatedAtResult($"/api/pagos/clientes/{pago.Id}");
    }
}
