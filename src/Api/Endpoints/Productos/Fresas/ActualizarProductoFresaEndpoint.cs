using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Endpoints.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Fresas;

public class ActualizarProductoFresaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/productos/fresa/{id:int}", Manejador)
           .WithName("ActualizarProductoFresa")
           .WithTags("Productos Fresa")
           .WithSummary("Actualiza la información de un producto fresa");
    }

    private static IResult Manejador(int id, ActualizarProductoFresaRequest request)
    {
        var index = ProductoDataStore.FresasDb.FindIndex(p => p.Id == id);
        if (index == -1)
        {
            return Result.Failure<ProductoFresaResponse>(Error.NotFound(
                "ProductoFresa.NotFound", $"No se encontró la fresa con ID: {id}"))
                .ToHttpResult();
        }

        var result = ProductoFresa.Create(
            request.Calibre,
            request.Calidad,
            request.Peso,
            request.PrecioCompra,
            request.Descripcion,
            id);

        if (result.IsFailure)
            return Result.Failure<ProductoFresaResponse>(result.Error).ToHttpResult();

        ProductoDataStore.FresasDb[index] = result.Value;

        var stock = InventarioDataStore.ObtenerPorProductoId(id)?.CantidadDisponible ?? 0;

        var response = new ProductoFresaResponse(
            result.Value.Id,
            result.Value.TipoProducto,
            result.Value.Calibre,
            result.Value.Calidad,
            result.Value.Peso,
            result.Value.PrecioCompra,
            result.Value.FechaIngreso,
            result.Value.Descripcion,
            stock
        );

        return Result.Success(response).ToHttpResult();
    }
}