using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

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
                        $"No se encontró la fresa con ID: {id}"))
                .ToHttpResult();
        }

        var result = fresa.Actualizar(
            request.Calibre,
            request.Calidad,
            request.Peso,
            request.PrecioCompra,
            request.Descripcion);

        if (result.IsFailure)
        {
            return Result
                .Failure<ProductoFresaResponse>(result.Error)
                .ToHttpResult();
        }

        var response = new ProductoFresaResponse(
            fresa.Id,
            fresa.TipoProducto,
            fresa.Calibre.Value,
            fresa.Calidad,
            fresa.Peso.Value,
            fresa.PrecioCompra.Value,
            fresa.FechaIngreso,
            fresa.Descripcion
        );

        return Result
            .Success(response)
            .ToHttpResult();
    }
}