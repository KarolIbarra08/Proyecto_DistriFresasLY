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

        var pagoActual = PagoDataStore.PagosProveedoresDb[index];

        var pagoActualizado = pagoActual with
        {
            ProveedorId = request.ProveedorId ?? pagoActual.ProveedorId,
            Valor = request.Valor ?? pagoActual.Valor,
            MedioPago = !string.IsNullOrWhiteSpace(request.MedioPago) ? request.MedioPago.Trim() : pagoActual.MedioPago,
            FechaInicial = request.FechaInicial ?? pagoActual.FechaInicial,
            FechaFinal = request.FechaFinal ?? pagoActual.FechaFinal,
            Estado = !string.IsNullOrWhiteSpace(request.Estado) ? request.Estado.Trim() : pagoActual.Estado
        };

        PagoDataStore.PagosProveedoresDb[index] = pagoActualizado;

        Result<PagoProveedorResponse> successResult = Result.Success(pagoActualizado);
        return successResult.ToHttpResult();
    }
}