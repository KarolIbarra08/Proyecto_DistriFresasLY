using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumos;

public class ConsultarProductosInsumoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/productos/insumo", Manejador)
           .WithName("ConsultarProductosInsumo")
           .WithTags("Productos Insumo")
           .WithSummary("Consulta todos los productos insumo");
    }

    private static IResult Manejador()
    {
        var lista = ProductoDataStore.InsumosDb
            .Select(insumo => new ProductoInsumoResponse(
                insumo.Id,
                insumo.TipoProducto,
                insumo.Nombre,
                insumo.Tipo,
                insumo.UnidadMedida,
                insumo.Descripcion,
                insumo.FechaIngreso
            ))
            .ToList();

        return Result
            .Success(lista)
            .ToHttpResult();
    }
}