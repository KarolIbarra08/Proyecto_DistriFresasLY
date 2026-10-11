using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos.Clientes;

public class ConsultarPagoClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/pagos/clientes/{id:int}", Manejador)
           .WithName("ConsultarPagoCliente")
           .WithTags("Pagos - Clientes")
           .WithSummary("Consulta un pago de cliente por ID");
    }

    private static IResult Manejador(int id)
    {
        var pago = PagoDataStore.PagosClientesDb.FirstOrDefault(p => p.Id == id);

        if (pago is null)
        {
            Result<PagoClienteResponse> notFoundResult = Error.NotFound(
                "PagoCliente.NotFound",
                $"No se encontro un pago de cliente registrado con el ID '{id}'.");

            return notFoundResult.ToHttpResult();
        }

        var response = new PagoClienteResponse(
            pago.Id,
            pago.ClienteId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaInicial,
            pago.FechaFinal,
            pago.Estado
        );

        Result<PagoClienteResponse> successResult = Result.Success(response);
        return successResult.ToHttpResult();
    }
}