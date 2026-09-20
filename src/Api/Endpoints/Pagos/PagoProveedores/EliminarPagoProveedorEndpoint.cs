using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Extensions;
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

    private static IResult Manejador(int id)
    {
        var pago = PagoDataStore.PagosProveedoresDb.FirstOrDefault(p => p.Id == id);

        if (pago is null)
        {
            Result<bool> notFoundResult = Error.NotFound(
                "PagoProveedor.NotFound",
                $"No se encontro un pago de proveedor registrado con el ID '{id}'.");

            return notFoundResult.ToHttpResult();
        }

        PagoDataStore.PagosProveedoresDb.Remove(pago);

        Result<bool> successResult = Result.Success(true);
        return successResult.ToHttpResult();
    }
}