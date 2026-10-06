using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos.Clientes;

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
        var index = PagoDataStore.PagosClientesDb.FindIndex(p => p.Id == id);

        if (index == -1)
        {
            Result<PagoClienteResponse> notFoundResult = Error.NotFound(
                "PagoCliente.NotFound",
                $"No se encontro un pago de cliente registrado con el ID '{id}'.");

            return notFoundResult.ToHttpResult();
        }

        var pagoActual = PagoDataStore.PagosClientesDb[index];

        var pagoActualizado = pagoActual with
        {
            ClienteId = request.ClienteId ?? pagoActual.ClienteId,
            Valor = request.Valor ?? pagoActual.Valor,
            MedioPago = !string.IsNullOrWhiteSpace(request.MedioPago) ? request.MedioPago.Trim() : pagoActual.MedioPago,
            FechaInicial = request.FechaInicial ?? pagoActual.FechaInicial,
            FechaFinal = request.FechaFinal ?? pagoActual.FechaFinal,
            Estado = !string.IsNullOrWhiteSpace(request.Estado) ? request.Estado.Trim() : pagoActual.Estado
        };

        PagoDataStore.PagosClientesDb[index] = pagoActualizado;

        Result<PagoClienteResponse> successResult = Result.Success(pagoActualizado);
        return successResult.ToHttpResult();
    }
}