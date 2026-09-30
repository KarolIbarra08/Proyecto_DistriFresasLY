using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Endpoints.Inventario;
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
           .WithSummary("Obtiene todos los productos fresa");
    }

    private static IResult Manejador()
    {
        var lista = ProductoDataStore.FresasDb.Select(f =>
        {
            var stock = InventarioDataStore.ObtenerPorProductoId(f.Id)?.CantidadDisponible ?? 0;
            return new ProductoFresaResponse(
                f.Id,
                f.TipoProducto,
                f.Calibre,
                f.Calidad,
                f.Peso,
                f.PrecioCompra,
                f.FechaIngreso,
                f.Descripcion,
                stock
            );
        }).ToList();

        return Result.Success(lista).ToHttpResult();
    }
}