using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        int id,
        IPagoClienteRepository pagoRepository,
        CancellationToken cancellationToken)
    {
        var pago = await pagoRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (pago is null)
        {
            Result<bool> notFoundResult = Error.NotFound(
                "PagoCliente.NotFound",
                $"No se encontro un pago de cliente registrado con el ID '{id}'.");

            return notFoundResult.ToHttpResult();
        }

        await pagoRepository.EliminarAsync(id, cancellationToken);

        return Result.Success(true).ToHttpResult();
    }
}
