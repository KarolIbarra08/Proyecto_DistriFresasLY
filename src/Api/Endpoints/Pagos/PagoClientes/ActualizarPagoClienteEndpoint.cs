using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Api.Endpoints.Pagos.PagoClientes;

public class ActualizarPagoClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/pagos/clientes/{id:int}", Manejador)
           .WithName("ActualizarPagoCliente")
           .WithTags("Pagos - Clientes")
           .WithSummary("Actualiza un pago de cliente existente");
    }

    private static IResult Manejador(int id, ActualizarPagoClienteRequest request)
    {
        var pago = PagoDataStore.PagosClientesDb.FirstOrDefault(p => p.Id == id);

        if (pago is null)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.NotFound("PagoCliente.NotFound", $"No se encontró el pago con ID {id}")).ToHttpResult();
        }

        var updateResult = pago.Actualizar(
            request.ClienteId,
            request.VentaId,
            request.Valor,
            request.MedioPago,
            request.FechaPago,
            request.Estado);

        if (updateResult.IsFailure)
        {
            return updateResult.ToHttpResult();
        }

        var response = new PagoClienteResponse(
            pago.Id,
            pago.ClienteId,
            pago.VentaId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaPago,
            pago.Estado
        );

        return Result.Success(response).ToHttpResult();
    }
}