using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos.PagoClientes;

public class ConsultarPagoClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/pagos/clientes/{id:int}", Manejador)
           .WithName("ConsultarPagoCliente")
           .WithTags("Pagos - Clientes")
           .WithSummary("Consulta un pago de cliente por su ID");
    }

    private static IResult Manejador(int id)
    {
        var pago = PagoDataStore.PagosClientesDb.FirstOrDefault(p => p.Id == id);

        if (pago is null)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.NotFound("PagoCliente.NotFound", $"No se encontró el pago con ID {id}")).ToHttpResult();
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