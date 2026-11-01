using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        int id,
        IPagoProveedorRepository pagoRepository,
        CancellationToken cancellationToken)
    {
        var pago = await pagoRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (pago is null)
        {
            return Result.Failure<PagoProveedorResponse>(
                Error.NotFound("PagoProveedor.NotFound",
                    $"No se encontro un pago de proveedor registrado con el ID '{id}'."))
                .ToHttpResult();
        }

        var response = new PagoProveedorResponse(
            pago.Id,
            pago.ProveedorId,
            pago.PedidoId,
            pago.Valor,
            pago.MedioPago,
            pago.FechaPago,
            pago.Estado);

        return Result.Success(response).ToHttpResult();
    }
}
