using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Productos.Fresas;

public class ConsultarTodosLosProductosFresasEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/productos/fresa", Manejador)
           .WithName("ConsultarTodosLosProductosFresas")
           .WithTags("Productos Fresa")
           .WithSummary("Consulta todos los productos fresa");
    }

    private static IResult Manejador()
    {
        var lista = ProductoDataStore.FresasDb
            .Select(fresa => new ProductoFresaResponse(
                fresa.Id,
                fresa.TipoProducto,
                fresa.Calibre.Value,
                fresa.Calidad,
                fresa.Peso.Value,
                fresa.PrecioCompra.Value,
                fresa.FechaIngreso,
                fresa.Descripcion
            ))
            .ToList();

        return Result
            .Success(lista)
            .ToHttpResult();
    }
}