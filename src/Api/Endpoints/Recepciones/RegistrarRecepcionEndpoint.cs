using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Recepciones;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class RegistrarRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/recepciones", Manejador)
           .WithName("RegistrarRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Registra una nueva recepción de mercancía");
    }

    private static IResult Manejador(
        RegistrarRecepcionRequest request)
    {
        var nuevoId = RecepcionDataStore.GenerarNuevoId();

        var fechaReal =
            request.FechaRecepcion.HasValue &&
            request.FechaRecepcion.Value != default
                ? request.FechaRecepcion.Value
                : DateTime.UtcNow;

        var result = Recepcion.Create(
            request.ValorPago,
            nuevoId,
            fechaReal);

        if (result.IsFailure)
        {
            return Result.Failure<RecepcionResponse>(
                result.Error)
                .ToHttpResult();
        }

        var recepcion = result.Value;

        RecepcionDataStore.RecepcionesDb.Add(recepcion);

        var response = new RecepcionResponse(
            recepcion.Id,
            recepcion.FechaRecepcion,
            recepcion.ValorPago);

        return Result.Success(response)
            .ToHttpCreatedAtResult(
                $"/api/recepciones/{recepcion.Id}");
    }
}