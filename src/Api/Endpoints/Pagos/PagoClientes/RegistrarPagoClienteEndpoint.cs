using DistriFresasLY.Api.Contracts.Pagos.Clientes;
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
           .WithSummary("Registra un nuevo pago de cliente");
    }

    private static IResult Manejador(CrearPagoClienteRequest request)
    {
        var nuevoId = PagoDataStore.PagosClientesDb.Count != 0
            ? PagoDataStore.PagosClientesDb.Max(p => p.Id) + 1
            : 1;

        var estado = string.IsNullOrWhiteSpace(request.Estado) ? "Pendiente" : request.Estado.Trim();

        var pagoResult = PagoCliente.Create(
            clienteId: request.ClienteId,
            valor: request.Valor,
            medioPago: request.MedioPago,
            fechaInicial: request.FechaInicial ?? DateTime.Now,
            fechaFinal: request.FechaFinal ?? DateTime.Now,
            estado: estado,
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
            pago.Valor,
            pago.MedioPago,
            pago.FechaInicial,
            pago.FechaFinal,
            pago.Estado
        );

        Result<PagoClienteResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/pagos/clientes/{pago.Id}");
    }
}