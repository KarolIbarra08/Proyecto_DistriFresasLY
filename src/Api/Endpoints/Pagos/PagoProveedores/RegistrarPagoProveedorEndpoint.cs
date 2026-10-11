using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Api.Endpoints.Pagos.Proveedores;

public class RegistrarPagoProveedorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/pagos/proveedores", Manejador)
           .WithName("RegistrarPagoProveedor")
           .WithTags("Pagos - Proveedores")
           .WithSummary("Registra un nuevo pago a proveedor");
    }

    private static IResult Manejador(CrearPagoProveedorRequest request)
    {
        var nuevoId = PagoDataStore.PagosProveedoresDb.Count != 0
            ? PagoDataStore.PagosProveedoresDb.Max(p => p.Id) + 1
            : 1;

        var estado = string.IsNullOrWhiteSpace(request.Estado) ? "Pendiente" : request.Estado.Trim();

        var pagoResult = PagoProveedor.Create(
            proveedorId: request.ProveedorId,
            valor: request.Valor,
            medioPago: request.MedioPago,
            fechaInicial: request.FechaInicial ?? DateTime.Now,
            fechaFinal: request.FechaFinal ?? DateTime.Now,
            estado: estado,
            id: nuevoId);

        if (pagoResult.IsFailure)
        {
            return Result.Failure<PagoProveedorResponse>(pagoResult.Error).ToHttpResult();
        }

        var pago = pagoResult.Value;
        PagoDataStore.PagosProveedoresDb.Add(pago);

        var response = new PagoProveedorResponse(
            pago.Id,
            pago.ProveedorId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaInicial,
            pago.FechaFinal,
            pago.Estado
        );

        Result<PagoProveedorResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/pagos/proveedores/{pago.Id}");
    }
}