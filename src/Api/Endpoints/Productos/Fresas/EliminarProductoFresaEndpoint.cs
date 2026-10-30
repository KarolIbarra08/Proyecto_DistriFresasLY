using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Fresas;

public class EliminarProductoFresaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/productos/fresa/{id:int}", Manejador)
           .WithName("EliminarProductoFresa")
           .WithTags("Productos Fresa")
           .WithSummary("Elimina un producto fresa por ID");
    }

    private static async Task<IResult> Manejador(
        int id,
        IProductoRepository productoRepository,
        CancellationToken cancellationToken)
    {

        var productoExistente = await productoRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (productoExistente is not ProductoFresa)
        {
            return Result.Failure(
                Error.NotFound("ProductoFresa.NotFound", $"No se encontró la fresa con ID: {id}")
            ).ToHttpResult();
        }


        await productoRepository.EliminarAsync(id, cancellationToken);


        var respuesta = new 
        { 
            mensaje = $"El producto fresa con ID {id} se eliminó correctamente." 
        };

        return Result.Success(respuesta).ToHttpResult();
    }
}