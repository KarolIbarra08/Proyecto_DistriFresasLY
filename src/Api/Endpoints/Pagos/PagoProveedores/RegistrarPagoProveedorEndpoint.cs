using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Endpoints.Pagos;
using DistriFresasLY.Api.Endpoints.Pedidos;
using DistriFresasLY.Api.Extensions;
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

    private static IResult Manejador(CrearPagoProveedorRequest request)
    {
        var pedido = PedidoDataStore.PedidosDb.FirstOrDefault(p => p.Id == request.PedidoId);
        if (pedido is null)
        {
            return Result.Failure<PagoProveedorResponse>(
                Error.NotFound("Pedido.NotFound", $"No existe el pedido con ID {request.PedidoId}")).ToHttpResult();
        }

        if (pedido.ProveedorId != request.ProveedorId)
        {
            return Result.Failure<PagoProveedorResponse>(
                Error.Validation("PagoProveedor.ProveedorNoCoincide", "El proveedor del pago no coincide con el proveedor del pedido.")).ToHttpResult();
        }

        decimal pagadoAnteriormente = PagoDataStore.PagosProveedoresDb
            .Where(p => p.PedidoId == request.PedidoId && p.Estado != "Anulado")
            .Sum(p => p.Valor);

        decimal saldoPendiente = pedido.Total - pagadoAnteriormente;

        if (saldoPendiente <= 0)
        {
            return Result.Failure<PagoProveedorResponse>(
                Error.Validation("PagoProveedor.PedidoSaldado", "Este pedido ya se encuentra totalmente pagado.")).ToHttpResult();
        }

        if (request.Valor > saldoPendiente)
        {
            return Result.Failure<PagoProveedorResponse>(
                Error.Validation("PagoProveedor.MontoExcedido",
                    $"El pago (${request.Valor}) supera el saldo pendiente del pedido (${saldoPendiente}).")).ToHttpResult();
        }

        decimal nuevoTotalPagado = pagadoAnteriormente + request.Valor;
        string estadoCalculado = (nuevoTotalPagado >= pedido.Total) ? "Completado" : "Abono Parcial";

        string estadoFinal = string.IsNullOrWhiteSpace(request.Estado) ? estadoCalculado : request.Estado.Trim();

        var nuevoId = PagoDataStore.PagosProveedoresDb.Count != 0
            ? PagoDataStore.PagosProveedoresDb.Max(p => p.Id) + 1
            : 1;

        var pagoResult = PagoProveedor.Create(
            proveedorId: request.ProveedorId,
            pedidoId: request.PedidoId,
            valor: request.Valor,
            medioPago: request.MedioPago,
            fechaPago: request.FechaPago,
            estado: estadoFinal,
            id: nuevoId);

        if (pagoResult.IsFailure)
        {
            return Result.Failure<PagoProveedorResponse>(pagoResult.Error).ToHttpResult();
        }

        var pago = pagoResult.Value;
        PagoDataStore.PagosProveedoresDb.Add(pago);

        var response = new PagoProveedorResponse(
            pago.Id,
            pago.ProveedorId,
            pago.PedidoId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaPago,
            pago.Estado
        );

        Result<PagoProveedorResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/pagos/proveedores/{pago.Id}");
    }
}
