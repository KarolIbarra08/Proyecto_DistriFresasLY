using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

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
    if (request.Valor <= 0)
    {
        Result<PagoProveedorResponse> errorResult = Error.Validation(
            "PagoProveedor.ValorInvalido", 
            "El valor del pago debe ser mayor a cero.");

        return errorResult.ToHttpResult();
    }

    var nuevoId = PagoDataStore.PagosProveedoresDb.Count != 0 
        ? PagoDataStore.PagosProveedoresDb.Max(p => p.Id) + 1 
        : 1;

    var nuevoPago = new PagoProveedorResponse(
        Id: nuevoId,
        ProveedorId: request.ProveedorId,
        Valor: request.Valor,
        MedioPago: request.MedioPago?.Trim() ?? string.Empty,
        FechaInicial: request.FechaInicial ?? DateTime.Now,
        FechaFinal: request.FechaFinal ?? DateTime.Now,
        Estado: string.IsNullOrWhiteSpace(request.Estado) ? "Pendiente" : request.Estado.Trim()
    );

    PagoDataStore.PagosProveedoresDb.Add(nuevoPago);

    Result<PagoProveedorResponse> createdResult = Result.Success(nuevoPago);
    return createdResult.ToHttpCreatedAtResult($"/api/pagos/proveedores/{nuevoPago.Id}");
}
}