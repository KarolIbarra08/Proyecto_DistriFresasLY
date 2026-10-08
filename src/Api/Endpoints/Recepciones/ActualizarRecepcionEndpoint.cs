using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Recepciones;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ActualizarRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/recepciones/{id:int}", Manejador)
           .WithName("ActualizarRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Actualiza una recepción existente");
    }

    private static IResult Manejador(int id, ActualizarRecepcionRequest request)
    {
        var index = RecepcionDataStore.RecepcionesDb.FindIndex(r => r.Id == id);
        if (index == -1)
        {
            return Result.Failure<RecepcionResponse>(Error.NotFound(
                "Recepcion.NotFound", $"No se encontró la recepción con ID: {id}"))
                .ToHttpResult();
        }

        var recepcionExistente = RecepcionDataStore.RecepcionesDb[index];

        var fechaReal = request.FechaRecepcion.HasValue 
            && request.FechaRecepcion.Value != default 
            && request.FechaRecepcion.Value.Year > 1970
                ? request.FechaRecepcion.Value
                : recepcionExistente.FechaRecepcion;

        var result = Recepcion.Create(
            request.Cantidad,
            request.ValorPago,
            id,
            fechaReal);

        if (result.IsFailure)
            return Result.Failure<RecepcionResponse>(result.Error).ToHttpResult();

        var entidadActualizada = result.Value;

        var response = new RecepcionResponse(
            entidadActualizada.Id,
            entidadActualizada.FechaRecepcion,
            entidadActualizada.Cantidad,
            entidadActualizada.ValorPago
        );

        RecepcionDataStore.RecepcionesDb[index] = response;

        return Result.Success(response).ToHttpResult();
    }
}