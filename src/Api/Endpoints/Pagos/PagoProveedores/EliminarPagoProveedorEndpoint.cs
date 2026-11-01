using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos.Proveedores;

public class EliminarPagoProveedorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/pagos/proveedores/{id:int}", Manejador)
           .WithName("EliminarPagoProveedor")
           .WithTags("Pagos - Proveedores")
           .WithSummary("Elimina un pago de proveedor por ID");
    }

    private static async Task<IResult> Manejador(
        int id,
        IPagoProveedorRepository pagoRepository,
        CancellationToken cancellationToken)
    {
        var pago = await pagoRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (pago is null)
        {
            Result<bool> notFoundResult = Error.NotFound(
                "PagoProveedor.NotFound",
                $"No se encontro un pago de proveedor registrado con el ID '{id}'.");

            return notFoundResult.ToHttpResult();
        }

        await pagoRepository.EliminarAsync(id, cancellationToken);

        return Result.Success(true).ToHttpResult();
    }
}
