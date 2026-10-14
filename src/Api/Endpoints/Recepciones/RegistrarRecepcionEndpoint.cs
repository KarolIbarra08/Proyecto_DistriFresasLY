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
           .WithSummary("Registra una nueva recepción");
    }

    private static IResult Manejador(RegistrarRecepcionRequest request)
    {
        var nuevoId = RecepcionDataStore.RecepcionesDb.Count != 0
            ? RecepcionDataStore.RecepcionesDb.Max(r => r.Id) + 1
            : 1;

        var result = Recepcion.Create(
            request.IdProveedor,
            nuevoId,
            request.FechaRecepcion);

        if (result.IsFailure)
        {
            return Result.Failure<RecepcionResponse>(result.Error).ToHttpResult();
        }

        var recepcion = result.Value;
        RecepcionDataStore.RecepcionesDb.Add(recepcion);

        var response = new RecepcionResponse(
            recepcion.Id,
            recepcion.IdProveedor,
            recepcion.FechaRecepcion,
            recepcion.ValorPago,
            recepcion.ProcesadaEnInventario,
            recepcion.CalcularTotal());

        return Result.Success(response)
            .ToHttpCreatedAtResult($"/api/recepciones/{recepcion.Id}");
    }
}