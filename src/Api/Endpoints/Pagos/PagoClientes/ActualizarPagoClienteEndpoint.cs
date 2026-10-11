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

        var pago = PagoDataStore.PagosClientesDb[index];

        var actualizarResult = pago.Actualizar(
            clienteId: request.ClienteId,
            valor: request.Valor,
            medioPago: request.MedioPago,
            fechaInicial: request.FechaInicial,
            fechaFinal: request.FechaFinal,
            estado: request.Estado);

        if (actualizarResult.IsFailure)
        {
            return Result.Failure<PagoClienteResponse>(actualizarResult.Error).ToHttpResult();
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

        return Result.Success(response).ToHttpResult();
    }
}