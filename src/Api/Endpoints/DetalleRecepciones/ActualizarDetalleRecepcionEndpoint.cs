using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.DetalleRecepciones;

public class ActualizarDetalleRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/detalles-recepcion/{id:int}", Manejador)
           .WithName("ActualizarDetalleRecepcion")
           .WithTags("DetalleRecepciones")
           .WithSummary("Actualiza un detalle de recepción");
    }

    private static async Task<IResult> Manejador(
        int id,
        ActualizarDetalleRecepcionRequest request,
        IRecepcionRepository recepcionRepository,
        CancellationToken cancellationToken)
    {
  
        if (request.Cantidad <= 0)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.Validation("DetalleRecepcion.CantidadInvalida", "La cantidad debe ser mayor a 0."))
                .ToHttpResult();
        }

        if (request.PrecioUnitario <= 0)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.Validation("DetalleRecepcion.PrecioInvalido", "El precio unitario debe ser mayor a 0."))
                .ToHttpResult();
        }

        var recepciones = await recepcionRepository.ObtenerTodosAsync(cancellationToken);

        var recepcion = recepciones.FirstOrDefault(r => r.Detalles.Any(d => d.Id == id));
        if (recepcion is null)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.NotFound("DetalleRecepcion.NotFound",
                    $"No se encontró el detalle con ID: {id}"))
                .ToHttpResult();
        }

       
        if (recepcion.ProcesadaEnInventario)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.Validation("DetalleRecepcion.RecepcionProcesada",
                    "No se pueden modificar detalles de una recepción que ya fue procesada en inventario."))
                .ToHttpResult();
        }

        var detalle = recepcion.Detalles.First(d => d.Id == id);
        var idProducto = detalle.IdProducto;

        var updateResult = detalle.Actualizar(request.Cantidad, request.PrecioUnitario);
        if (updateResult.IsFailure)
        {
            return Result.Failure<DetalleRecepcionResponse>(updateResult.Error).ToHttpResult();
        }

        await recepcionRepository.ActualizarAsync(recepcion, cancellationToken);

        var guardada = await recepcionRepository.ObtenerPorIdAsync(recepcion.Id, cancellationToken);
        var actualizado = guardada!.Detalles.LastOrDefault(d => d.IdProducto == idProducto);

        if (actualizado is null)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.Failure("DetalleRecepcion.ErrorActualizacion", "No se pudo recuperar el detalle actualizado."))
                .ToHttpResult();
        }

        var response = new DetalleRecepcionResponse(
            actualizado.Id,
            actualizado.IdRecepcion,
            actualizado.IdProducto,
            (int)actualizado.Cantidad,
            actualizado.PrecioUnitario,
            actualizado.Subtotal,
            actualizado.FechaIngreso);

        return Result.Success(response).ToHttpResult();
    }
}