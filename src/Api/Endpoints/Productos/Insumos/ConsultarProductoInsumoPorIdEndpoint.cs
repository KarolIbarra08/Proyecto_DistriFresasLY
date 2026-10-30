using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumos;

public class ConsultarProductoInsumoPorIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/productos/insumo/{id:int}", Manejador)
           .WithName("ConsultarProductoInsumoPorId")
           .WithTags("Productos Insumo")
           .WithSummary("Consulta un producto insumo por ID");
    }

    private static async Task<IResult> Manejador(
        int id, 
        IProductoRepository productoRepository, 
        CancellationToken cancellationToken)
    {
       
        var producto = await productoRepository.ObtenerPorIdAsync(id, cancellationToken);

    
        if (producto is not ProductoInsumo insumo)
        {
            return Result
                .Failure<ProductoInsumoResponse>(
                    Error.NotFound(
                        "ProductoInsumo.NotFound",
                        $"No se encontró el insumo con ID: {id}"))
                .ToHttpResult();
        }


        var response = new ProductoInsumoResponse(
            insumo.Id,
            insumo.TipoProducto.ToString(),
            insumo.Nombre,
            insumo.TipoInsumo,
            insumo.UnidadMedida,
            insumo.Descripcion
        );

       
        return Result
            .Success(response)
            .ToHttpResult();
    }
}