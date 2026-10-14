using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Recepciones;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ObtenerRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones/{id:int}", Manejador)
           .WithName("ObtenerRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Obtiene una recepción por su Id");
    }

    private static IResult Manejador(int id)
    {
        var recepcion = RecepcionDataStore.RecepcionesDb.FirstOrDefault(r => r.Id == id);
        if (recepcion is null)
        {
            Result<RecepcionResponse> errorResult = Error.NotFound(
                "Recepcion.NotFound",
                $"No se encontró una recepción con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var response = new RecepcionResponse(
            recepcion.Id,
            recepcion.IdProveedor,
            recepcion.FechaRecepcion,
            recepcion.ValorPago,
            recepcion.ProcesadaEnInventario,
            recepcion.CalcularTotal());

        return Result.Success(response).ToHttpResult();
    }
}