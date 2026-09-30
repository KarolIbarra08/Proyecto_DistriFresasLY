using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumo;

public class ActualizarProductoInsumoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/productos/insumo/{id:int}", Manejador)
           .WithName("ActualizarProductoInsumo")
           .WithTags("Productos Insumo")
           .WithSummary("Actualiza un producto insumo");
    }

    private static IResult Manejador(int id, ActualizarProductoInsumoRequest request)
    {
        var index = ProductoDataStore.InsumosDb.FindIndex(p => p.Id == id);
        if (index == -1)
        {
            return Result.Failure<ProductoInsumoResponse>(Error.NotFound(
                "ProductoInsumo.NotFound", $"No se encontro el insumo con ID: {id}"))
                .ToHttpResult();
        }

        var result = ProductoInsumo.Create(
            request.Nombre,
            request.Tipo,
            request.UnidadMedida,
            request.Descripcion,
            id);

        if (result.IsFailure)
            return Result.Failure<ProductoInsumoResponse>(result.Error).ToHttpResult();

        ProductoDataStore.InsumosDb[index] = result.Value;

        var response = new ProductoInsumoResponse(
            result.Value.Id, result.Value.TipoProducto, result.Value.Nombre,
            result.Value.Tipo, result.Value.UnidadMedida, result.Value.Descripcion);

        return Result.Success(response).ToHttpResult();
    }
}