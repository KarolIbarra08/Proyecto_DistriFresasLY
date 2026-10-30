using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumos;

public class EliminarProductoInsumoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/productos/insumo/{id:int}", Manejador)
           .WithName("EliminarProductoInsumo")
           .WithTags("Productos Insumo")
           .WithSummary("Elimina un producto insumo por su ID");
    }

    private static async Task<IResult> Manejador(
        int id, 
        IProductoRepository productoRepository, 
        CancellationToken cancellationToken)
    {

        var productoExistente = await productoRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (productoExistente is not ProductoInsumo)
        {
            return Result.Failure(
                Error.NotFound("ProductoInsumo.NotFound", $"No se encontró el insumo con ID {id}")
            ).ToHttpResult();
        }


        await productoRepository.EliminarAsync(id, cancellationToken);


        var respuesta = new 
        { 
            mensaje = $"El producto insumo con ID {id} se eliminó correctamente." 
        };

        return Result.Success(respuesta).ToHttpResult();
    }
}