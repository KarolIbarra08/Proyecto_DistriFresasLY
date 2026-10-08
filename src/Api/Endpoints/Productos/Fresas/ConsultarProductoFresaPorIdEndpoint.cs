using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Productos.Fresas;

public class ConsultarProductoFresaPorIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/productos/fresa/{id:int}", Manejador)
           .WithName("ConsultarProductoFresaPorId")
           .WithTags("Productos Fresa")
           .WithSummary("Consulta un producto fresa por ID");
    }

    private static IResult Manejador(int id)
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