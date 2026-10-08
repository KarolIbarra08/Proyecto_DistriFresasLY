using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

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

    private static IResult Manejador(int id)
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