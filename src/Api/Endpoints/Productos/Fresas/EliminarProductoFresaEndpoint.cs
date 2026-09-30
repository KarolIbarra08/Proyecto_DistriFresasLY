using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Productos.Fresa;

public class EliminarProductoFresaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/productos/fresa/{id:int}", Manejador)
           .WithName("EliminarProductoFresa")
           .WithTags("Productos Fresa")
           .WithSummary("Elimina un producto fresa por ID");
    }

    private static IResult Manejador(int id)
    {
        var index = ProductoDataStore.FresasDb.FindIndex(p => p.Id == id);
        if (index == -1)
        {
            return Result.Failure<bool>(Error.NotFound(
                "ProductoFresa.NotFound", $"No se encontro la fresa con ID: {id}"))
                .ToHttpResult();
        }

        ProductoDataStore.FresasDb.RemoveAt(index);
        return Result.Success(true).ToHttpResult();
    }
}