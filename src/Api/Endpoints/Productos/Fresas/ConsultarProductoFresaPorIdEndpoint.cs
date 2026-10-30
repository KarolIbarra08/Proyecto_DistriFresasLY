using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Fresas;

public class ConsultarProductoFresaPorIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // DEBE SER MapGet, NO MapDelete
        app.MapGet("/productos/fresa/{id:int}", Manejador)
           .WithName("ConsultarProductoFresaPorId")
           .WithTags("Productos Fresa")
           .WithSummary("Consulta un producto fresa por ID");
    }

    private static async Task<IResult> Manejador(
        int id,
        IProductoRepository productoRepository,
        CancellationToken cancellationToken)
    {
        var producto = await productoRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (producto is not ProductoFresa fresa)
        {
            return Result
                .Failure<ProductoFresaResponse>(
                    Error.NotFound(
                        "ProductoFresa.NotFound",
                        $"No se encontró la fresa con ID: {id}"))
                .ToHttpResult();
        }

        var response = new ProductoFresaResponse(
            fresa.Id,
            fresa.TipoProducto.ToString(),
            fresa.Calibre.Value,
            fresa.Calidad,
            fresa.Peso.Value,
            fresa.Descripcion
        );

        return Result
            .Success(response)
            .ToHttpResult();
    }
}