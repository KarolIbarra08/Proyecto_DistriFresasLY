using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Endpoints.Inventario;
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
                "ProductoInsumo.NotFound", $"No se encontró el insumo con ID: {id}"))
                .ToHttpResult();
        }

        var insumoExistente = ProductoDataStore.InsumosDb[index];

        var result = ProductoInsumo.Create(
            request.Nombre,
            request.Tipo,
            request.UnidadMedida,
            request.Descripcion,
            id,
            insumoExistente.FechaIngreso);

        if (result.IsFailure)
            return Result.Failure<ProductoInsumoResponse>(result.Error).ToHttpResult();

        ProductoDataStore.InsumosDb[index] = result.Value;

        var stock = InventarioDataStore.ObtenerPorProductoId(id)?.CantidadDisponible ?? 0;

        var response = new ProductoInsumoResponse(
            result.Value.Id,
            result.Value.TipoProducto,
            result.Value.Nombre,
            result.Value.Tipo,
            result.Value.UnidadMedida,
            result.Value.Descripcion,
            result.Value.FechaIngreso,
            stock
        );

        return Result.Success(response).ToHttpResult();
    }
}