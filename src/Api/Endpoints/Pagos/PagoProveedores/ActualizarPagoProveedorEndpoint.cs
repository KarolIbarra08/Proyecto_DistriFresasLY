using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos.Proveedores;

public class ActualizarPagoProveedorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/pagos/proveedores/{id:int}", Manejador)
           .WithName("ActualizarPagoProveedor")
           .WithTags("Pagos - Proveedores")
           .WithSummary("Actualiza un pago a proveedor existente");
    }

    private static IResult Manejador(int id, ActualizarPagoProveedorRequest request)
    {
        var index = PagoDataStore.PagosProveedoresDb.FindIndex(p => p.Id == id);

        if (index == -1)
        {
            Result<PagoProveedorResponse> notFoundResult = Error.NotFound(
                "PagoProveedor.NotFound",
                $"No se encontro un pago de proveedor registrado con el ID '{id}'.");

            return notFoundResult.ToHttpResult();
        }

        var pago = PagoDataStore.PagosProveedoresDb[index];

        var actualizarResult = pago.Actualizar(
            proveedorId: request.ProveedorId,
            valor: request.Valor,
            medioPago: request.MedioPago,
            fechaInicial: request.FechaInicial,
            fechaFinal: request.FechaFinal,
            estado: request.Estado);

        if (actualizarResult.IsFailure)
        {
            return Result.Failure<PagoProveedorResponse>(actualizarResult.Error).ToHttpResult();
        }

        var response = new PagoProveedorResponse(
            pago.Id,
            pago.ProveedorId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaInicial,
            pago.FechaFinal,
            pago.Estado
        );

        return Result.Success(response).ToHttpResult();
    }
}