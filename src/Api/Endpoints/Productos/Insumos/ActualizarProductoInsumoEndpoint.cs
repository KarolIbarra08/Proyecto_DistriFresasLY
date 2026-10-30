using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumos;

public class ActualizarProductoInsumoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/productos/insumo/{id:int}", Manejador)
           .WithName("ActualizarProductoInsumo")
           .WithTags("Productos Insumo")
           .WithSummary("Actualiza los datos de un producto insumo y retorna el resultado");
    }

    private static async Task<IResult> Manejador(
        int id, 
        ActualizarProductoInsumoRequest request, 
        IProductoRepository productoRepository, 
        CancellationToken cancellationToken)
    {

        var productoExistente = await productoRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (productoExistente is not ProductoInsumo insumo)
        {
            return Result.Failure<ProductoInsumoResponse>(
                Error.NotFound("ProductoInsumo.NotFound", $"No se encontró el insumo con ID {id}")
            ).ToHttpResult();
        }


        var nombreNuevo = string.IsNullOrWhiteSpace(request.Nombre) ? insumo.Nombre : request.Nombre;
        var tipoInsumoNuevo = string.IsNullOrWhiteSpace(request.TipoInsumo) ? insumo.TipoInsumo : request.TipoInsumo;
        var unidadMedidaNueva = string.IsNullOrWhiteSpace(request.UnidadMedida) ? insumo.UnidadMedida : request.UnidadMedida;
        var descripcionNueva = request.Descripcion ?? insumo.Descripcion;


        var updateResult = insumo.ActualizarDatos(
            nombreNuevo,
            tipoInsumoNuevo,
            unidadMedidaNueva,
            descripcionNueva);

        if (updateResult.IsFailure)
        {
            return Result.Failure<ProductoInsumoResponse>(updateResult.Error).ToHttpResult();
        }


        await productoRepository.ActualizarInsumoAsync(insumo, cancellationToken);


        var response = new ProductoInsumoResponse(
            insumo.Id,
            insumo.TipoProducto.ToString(),
            insumo.Nombre,
            insumo.TipoInsumo,
            insumo.UnidadMedida,
            insumo.Descripcion
        );

        return Result.Success(response).ToHttpResult();
    }
}