using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Proveedores;

public class ObtenerProveedoresEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/proveedores", Manejador)
           .WithName("ObtenerProveedores")
           .WithTags("Proveedores")
           .WithSummary("Obtiene la lista de proveedores registrados");
    }

    private static IResult Manejador()
    {
        var result = Result.Success(ProveedorDataStore.ProveedoresDb.AsReadOnly());
        return result.ToHttpResult();
    }
}