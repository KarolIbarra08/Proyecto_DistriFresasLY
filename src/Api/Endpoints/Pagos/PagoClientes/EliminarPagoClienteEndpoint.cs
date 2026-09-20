using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos.Clientes;

public class EliminarPagoClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/pagos/clientes/{id:int}", Manejador)
           .WithName("EliminarPagoCliente")
           .WithTags("Pagos - Clientes")
           .WithSummary("Elimina un pago de cliente por ID");
    }

    private static IResult Manejador(int id)
    {
        var pago = PagoDataStore.PagosClientesDb.FirstOrDefault(p => p.Id == id);

        if (pago is null)
        {
            Result<bool> notFoundResult = Error.NotFound(
                "PagoCliente.NotFound",
                $"No se encontro un pago de cliente registrado con el ID '{id}'.");

            return notFoundResult.ToHttpResult();
        }

        PagoDataStore.PagosClientesDb.Remove(pago);

        Result<bool> successResult = Result.Success(true);
        return successResult.ToHttpResult();
    }
}