using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Productos.Fresas;

public class ConsultarTodosLosProductosFresasEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/productos/fresa", Manejador)
           .WithName("ConsultarTodosLosProductosFresas")
           .WithTags("Productos Fresa")
           .WithSummary("Consulta todos los productos fresa");
    }

    private static async Task<IResult> Manejador(
        IProductoRepository productoRepository,
        CancellationToken cancellationToken)
    {

        var fresasEntities = await productoRepository.ObtenerTodasFresasAsync(cancellationToken);


        var lista = fresasEntities
            .Select(fresa => new ProductoFresaResponse(
                fresa.Id,
                fresa.TipoProducto.ToString(),
                fresa.Calibre.Value,
                fresa.Calidad,
                fresa.Peso.Value,
                fresa.Descripcion
            ))
            .ToList();

        return Result
            .Success(lista)
            .ToHttpResult();
    }
}