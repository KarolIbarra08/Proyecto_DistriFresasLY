using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumo;

public class EliminarProductoInsumoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/productos/insumo/{id:int}", Manejador)
           .WithName("EliminarProductoInsumo")
           .WithTags("Productos Insumo")
           .WithSummary("Elimina un producto insumo por ID");
    }

    private static IResult Manejador(int id)
    {
        var index = ProductoDataStore.InsumosDb.FindIndex(p => p.Id == id);
        if (index == -1)
        {
            return Result.Failure<bool>(Error.NotFound(
                "ProductoInsumo.NotFound", $"No se encontro el insumo con ID: {id}"))
                .ToHttpResult();
        }

        ProductoDataStore.InsumosDb.RemoveAt(index);
        return Result.Success(true).ToHttpResult();
    }
}