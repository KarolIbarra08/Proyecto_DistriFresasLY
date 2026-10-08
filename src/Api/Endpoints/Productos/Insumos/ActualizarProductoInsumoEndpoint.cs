using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumos;

public class ActualizarProductoInsumoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/productos/insumo/{id:int}", Manejador)
           .WithName("ActualizarProductoInsumo")
           .WithTags("Productos Insumo")
           .WithSummary("Actualiza la información de un producto insumo");
    }

    private static IResult Manejador(
        int id,
        ActualizarProductoInsumoRequest request)
    {
        var insumo = ProductoDataStore.InsumosDb
            .FirstOrDefault(p => p.Id == id);

        if (insumo is null)
        {
            return Result
                .Failure<ProductoInsumoResponse>(
                    Error.NotFound(
                        "ProductoInsumo.NotFound",
                        $"No se encontró el insumo con ID: {id}"))
                .ToHttpResult();
        }

        var result = insumo.Actualizar(
            request.Nombre,
            request.Tipo,
            request.UnidadMedida,
            request.Descripcion);

        if (result.IsFailure)
        {
            return Result
                .Failure<ProductoInsumoResponse>(result.Error)
                .ToHttpResult();
        }

        var response = new ProductoInsumoResponse(
            insumo.Id,
            insumo.TipoProducto,
            insumo.Nombre,
            insumo.Tipo,
            insumo.UnidadMedida,
            insumo.Descripcion,
            insumo.FechaIngreso
        );

        return Result
            .Success(response)
            .ToHttpResult();
    }
}