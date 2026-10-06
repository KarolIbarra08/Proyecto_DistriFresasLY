using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos.Proveedores;

public class ConsultarPagoProveedorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/pagos/proveedores/{id:int}", Manejador)
           .WithName("ConsultarPagoProveedor")
           .WithTags("Pagos - Proveedores")
           .WithSummary("Consulta un pago de proveedor por ID");
    }

    private static IResult Manejador(int id)
    {
        var pago = PagoDataStore.PagosProveedoresDb.FirstOrDefault(p => p.Id == id);

        if (pago is null)
        {
            Result<PagoProveedorResponse> notFoundResult = Error.NotFound(
                "PagoProveedor.NotFound",
                $"No se encontro un pago de proveedor registrado con el ID '{id}'.");

            return notFoundResult.ToHttpResult();
        }

        Result<PagoProveedorResponse> successResult = Result.Success(pago);
        return successResult.ToHttpResult();
    }
}