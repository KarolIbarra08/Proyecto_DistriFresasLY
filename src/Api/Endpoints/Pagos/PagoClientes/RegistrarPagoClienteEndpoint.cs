using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Endpoints.Ventas;
using DistriFresasLY.Api.Extensions;
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

    private static IResult Manejador(CrearPagoClienteRequest request)
    {
       
        var venta = VentaDataStore.VentasDb.FirstOrDefault(v => v.Id == request.VentaId);
        if (venta is null)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.NotFound("Venta.NotFound", $"No existe la venta con ID {request.VentaId}")).ToHttpResult();
        }


        if (venta.ClienteId != request.ClienteId)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation("PagoCliente.ClienteNoCoincide", "El ClienteID no coincide con el cliente de la venta.")).ToHttpResult();
        }


        decimal pagadoAnteriormente = PagoDataStore.PagosClientesDb
            .Where(p => p.VentaId == request.VentaId && p.Estado != "Anulado")
            .Sum(p => p.Valor);

        decimal saldoPendiente = venta.Total - pagadoAnteriormente;

        if (saldoPendiente <= 0)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation("PagoCliente.VentaSaldada", "Esta venta ya se encuentra totalmente pagada.")).ToHttpResult();
        }

        if (request.Valor > saldoPendiente)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.Validation("PagoCliente.MontoExcedido", 
                    $"El abono (${request.Valor}) supera el saldo pendiente de la venta (${saldoPendiente}).")).ToHttpResult();
        }


        decimal nuevoTotalPagado = pagadoAnteriormente + request.Valor;
        string estadoCalculado = (nuevoTotalPagado >= venta.Total) ? "Completado" : "Abono Parcial";

      
        string estadoFinal = string.IsNullOrWhiteSpace(request.Estado) ? estadoCalculado : request.Estado.Trim();

        var nuevoId = PagoDataStore.PagosClientesDb.Count != 0
            ? PagoDataStore.PagosClientesDb.Max(p => p.Id) + 1
            : 1;


        var pagoResult = PagoCliente.Create(
            clienteId: request.ClienteId,
            ventaId: request.VentaId,
            valor: request.Valor,
            medioPago: request.MedioPago,
            fechaPago: request.FechaPago,
            estado: estadoFinal,
            id: nuevoId);

        if (pagoResult.IsFailure)
        {
            return Result.Failure<PagoClienteResponse>(pagoResult.Error).ToHttpResult();
        }

        var pago = pagoResult.Value;
        PagoDataStore.PagosClientesDb.Add(pago);

        var response = new PagoClienteResponse(
            pago.Id,
            pago.ClienteId,
            pago.VentaId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaPago,
            pago.Estado
        );

        Result<PagoClienteResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/pagos/clientes/{pago.Id}");
    }
}