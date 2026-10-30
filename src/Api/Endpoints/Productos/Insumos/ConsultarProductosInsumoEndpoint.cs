using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumos;

public class ConsultarProductosInsumoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/productos/insumo", Manejador)
           .WithName("ConsultarProductosInsumo")
           .WithTags("Productos Insumo")
           .WithSummary("Consulta la lista de productos insumo");
    }

    private static async Task<IResult> Manejador(
        IProductoRepository productoRepository,
        CancellationToken cancellationToken)
    {
      
        var insumosEntities = await productoRepository.ObtenerTodosInsumosAsync(cancellationToken);

    
        var insumosResponse = insumosEntities
            .Select(insumo => new ProductoInsumoResponse(
                insumo.Id,
                insumo.TipoProducto.ToString(),
                insumo.Nombre,
                insumo.TipoInsumo,
                insumo.UnidadMedida,
                insumo.Descripcion
            ))
            .ToList();

        
        return Result.Success(insumosResponse).ToHttpResult();
    }
}