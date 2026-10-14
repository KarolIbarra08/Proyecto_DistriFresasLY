using DistriFresasLY.Api.Contracts.Productos;
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
           .WithSummary("Actualiza un producto fresa existente");
    }

    private static IResult Manejador(
        int id,
        ActualizarProductoFresaRequest request)
    {
        var fresa = ProductoDataStore.FresasDb
            .FirstOrDefault(p => p.Id == id);

        if (fresa is null)
        {
            return Result
                .Failure<ProductoFresaResponse>(
                    Error.NotFound(
                        "ProductoFresa.NotFound",
                        $"No se encontró el producto fresa con ID: {id}"))
                .ToHttpResult();
        }

        
        var updateResult = fresa.Actualizar(
            request.Calibre,
            request.Calidad,
            request.Peso,
            request.Descripcion,
            c => ProductoDataStore.FresasDb.Any(p => p.Id != id && p.Calibre.Value == c));

        if (updateResult.IsFailure)
        {
            return Result
                .Failure<ProductoFresaResponse>(updateResult.Error)
                .ToHttpResult();
        }

        var response = new ProductoFresaResponse(
            fresa.Id,
            fresa.TipoProducto,
            fresa.Calibre.Value,
            fresa.Calidad,
            fresa.Peso.Value,
            fresa.Descripcion
        );

        return Result
            .Success(response)
            .ToHttpResult();
    }
}