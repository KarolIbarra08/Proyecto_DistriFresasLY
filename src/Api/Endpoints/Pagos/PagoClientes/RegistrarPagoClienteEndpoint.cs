using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

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
    if (request.Valor <= 0)
    {
        Result<PagoClienteResponse> errorResult = Error.Validation(
            "PagoCliente.ValorInvalido", 
            "El valor del pago debe ser mayor a cero.");

        return errorResult.ToHttpResult();
    }

    var nuevoId = PagoDataStore.PagosClientesDb.Count != 0 
        ? PagoDataStore.PagosClientesDb.Max(p => p.Id) + 1 
        : 1;

    var nuevoPago = new PagoClienteResponse(
        Id: nuevoId,
        ClienteId: request.ClienteId,
        Valor: request.Valor,
        MedioPago: request.MedioPago?.Trim() ?? string.Empty,
        FechaInicial: request.FechaInicial ?? DateTime.Now,
        FechaFinal: request.FechaFinal ?? DateTime.Now,
        Estado: string.IsNullOrWhiteSpace(request.Estado) ? "Pendiente" : request.Estado.Trim()
    );

    PagoDataStore.PagosClientesDb.Add(nuevoPago);

    Result<PagoClienteResponse> createdResult = Result.Success(nuevoPago);
    return createdResult.ToHttpCreatedAtResult($"/api/pagos/clientes/{nuevoPago.Id}");
}
}