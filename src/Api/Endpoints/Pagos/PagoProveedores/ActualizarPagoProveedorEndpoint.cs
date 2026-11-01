using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        int id,
        ActualizarPagoProveedorRequest request,
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

        var actualizarResult = pago.Actualizar(
            proveedorId: request.ProveedorId,
            pedidoId: request.PedidoId,
            valor: request.Valor,
            medioPago: request.MedioPago,
            fechaPago: request.FechaPago,
            estado: request.Estado);

        if (actualizarResult.IsFailure)
        {
            return Result.Failure<PagoProveedorResponse>(actualizarResult.Error).ToHttpResult();
        }

        await pagoRepository.ActualizarAsync(pago, cancellationToken);

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
