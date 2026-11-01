using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

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

    private static async Task<IResult> Manejador(
        int id,
        ActualizarPagoClienteRequest request,
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

        var updateResult = pago.Actualizar(
            request.ClienteId,
            request.VentaId,
            request.Valor,
            request.MedioPago,
            request.FechaPago,
            request.Estado);

        if (updateResult.IsFailure)
        {
            return Result.Failure<PagoClienteResponse>(updateResult.Error).ToHttpResult();
        }

        await pagoRepository.ActualizarAsync(pago, cancellationToken);

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
