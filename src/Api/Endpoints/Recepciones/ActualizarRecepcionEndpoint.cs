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
           .WithSummary("Actualiza los datos de una recepción existente");
    }

    private static IResult Manejador(int id, ActualizarRecepcionRequest request)
    {
        var recepcion = RecepcionDataStore.RecepcionesDb.FirstOrDefault(r => r.Id == id);
        if (recepcion is null)
        {
            return Result.Failure<RecepcionResponse>(
                Error.NotFound("Recepcion.NotFound", $"No se encontró una recepción con el Id: {id}")
            ).ToHttpResult();
        }

        var idProveedor = request.IdProveedor > 0 ? request.IdProveedor : recepcion.IdProveedor;

        var actualizarResult = recepcion.Actualizar(idProveedor);

        if (actualizarResult.IsFailure)
        {
            return Result.Failure<RecepcionResponse>(actualizarResult.Error).ToHttpResult();
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