using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Endpoints.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Recepciones;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ProcesarRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/recepciones/{id:int}/procesar", Manejador)
           .WithName("ProcesarRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Marca una recepción como procesada e incrementa el inventario");
    }

    private static IResult Manejador(int id)
    {
        var recepcion = RecepcionDataStore.RecepcionesDb.FirstOrDefault(r => r.Id == id);

        if (recepcion is null)
        {
            return Result.Failure<RecepcionResponse>(
                Error.NotFound("Recepcion.NotFound", $"No se encontró una recepción con el Id: {id}")).ToHttpResult();
        }

        if (recepcion.ProcesadaEnInventario)
        {
            return Result.Failure<RecepcionResponse>(
                Error.Conflict("Recepcion.YaProcesada", "La recepción ya fue procesada en inventario.")).ToHttpResult();
        }

        if (!recepcion.Detalles.Any())
        {
            return Result.Failure<RecepcionResponse>(
                Error.Validation("Recepcion.SinDetalles", "No se puede procesar una recepción sin detalles.")).ToHttpResult();
        }

        var procesar = recepcion.MarcarComoProcesada();
        if (procesar.IsFailure)
        {
            return Result.Failure<RecepcionResponse>(procesar.Error).ToHttpResult();
        }

        // El inventario ya se actualiza al registrar cada DetalleRecepcion.
        // Aqui solo se marca la recepcion como procesada para evitar modificaciones posteriores.

        var response = new RecepcionResponse(
            recepcion.Id,
            recepcion.PedidoId,
            recepcion.FechaRecepcion,
            recepcion.ValorPago,
            recepcion.ProcesadaEnInventario,
            recepcion.CalcularTotal());

        return Result.Success(response).ToHttpResult();
    }
}
