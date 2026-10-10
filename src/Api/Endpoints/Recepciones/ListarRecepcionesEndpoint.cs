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
           .WithSummary("Muestra todas las recepciones");
    }

    private static IResult Manejador()
    {
        var response = RecepcionDataStore.RecepcionesDb
            .Select(r => new RecepcionResponse(
                r.Id,
                r.FechaRecepcion,
                r.ValorPago))
            .ToList();

        return Result.Success(response)
            .ToHttpResult();
    }
}