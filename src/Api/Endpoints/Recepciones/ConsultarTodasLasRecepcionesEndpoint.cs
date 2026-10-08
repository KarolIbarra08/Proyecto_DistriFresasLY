using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ConsultarTodasLasRecepcionesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones", Manejador)
           .WithName("ConsultarTodasLasRecepciones")
           .WithTags("Recepciones")
           .WithSummary("Obtiene todas las recepciones de mercancía");
    }

    private static IResult Manejador()
    {
        var lista = RecepcionDataStore.RecepcionesDb.ToList();
        return Result.Success(lista).ToHttpResult();
    }
}