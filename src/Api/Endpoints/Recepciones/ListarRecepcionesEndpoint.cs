using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ListarRecepcionesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones", Manejador)
           .WithName("ListarRecepciones")
           .WithTags("Recepciones")
           .WithSummary("Obtiene el listado general de recepciones");
    }

    private static IResult Manejador()
    {
        var lista = RecepcionDataStore.RecepcionesDb
            .Select(r => new RecepcionResponse(
                r.Id,
                r.IdProveedor,
                r.FechaRecepcion,
                r.ValorPago,
                r.ProcesadaEnInventario,
                r.CalcularTotal()))
            .ToList();

        return Result.Success(lista).ToHttpResult();
    }
}