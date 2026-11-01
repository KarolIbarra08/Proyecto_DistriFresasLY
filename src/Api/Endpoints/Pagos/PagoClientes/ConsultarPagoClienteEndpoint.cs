using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        int id,
        IPagoClienteRepository pagoRepository,
        CancellationToken cancellationToken)
    {
        var pago = await pagoRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (pago is null)
        {
            return Result.Failure<PagoClienteResponse>(
                Error.NotFound("PagoCliente.NotFound", $"No se encontró el pago con ID {id}"))
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

        return Result.Success(response).ToHttpResult();
    }
}
