using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Endpoints.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumo;

public class ConsultarProductosInsumoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/productos/insumo", Manejador)
           .WithName("ConsultarProductosInsumo")
           .WithTags("Productos Insumo")
           .WithSummary("Obtiene todos los productos insumo");
    }

    private static IResult Manejador()
    {
        var lista = ProductoDataStore.InsumosDb.Select(i => 
        {
            var stock = InventarioDataStore.ObtenerPorProductoId(i.Id)?.CantidadDisponible ?? 0;
            return new ProductoInsumoResponse(
                i.Id, 
                i.TipoProducto, 
                i.Nombre, 
                i.Tipo, 
                i.UnidadMedida, 
                i.Descripcion,
                i.FechaIngreso,
                stock
            );
        }).ToList();

        return Result.Success(lista).ToHttpResult();
    }
}