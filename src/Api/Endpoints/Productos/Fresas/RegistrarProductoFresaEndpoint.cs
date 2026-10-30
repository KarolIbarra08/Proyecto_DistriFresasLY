using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Fresas;

public class RegistrarProductoFresaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/productos/fresa", Manejador)
           .WithName("RegistrarProductoFresa")
           .WithTags("Productos Fresa")
           .WithSummary("Registra un nuevo producto fresa");
    }

    private static async Task<IResult> Manejador(
        CrearProductoFresaRequest request,
        IProductoRepository productoRepository,
        CancellationToken cancellationToken)
    {

        bool ExisteCalibre(int c) => productoRepository
            .ExisteCalibreAsync(c, idExcluir: null, cancellationToken)
            .GetAwaiter()
            .GetResult();

       
        var result = ProductoFresa.Create(
            request.Calibre,
            request.Calidad,
            request.Peso,
            request.Descripcion,
            ExisteCalibre,
            id: 0);

        if (result.IsFailure)
        {
            return Result
                .Failure<ProductoFresaResponse>(result.Error)
                .ToHttpResult();
        }

        var fresa = result.Value;


        var idGenerado = await productoRepository.AgregarFresaAsync(fresa, cancellationToken);

  
        var response = new ProductoFresaResponse(
            idGenerado,
            fresa.TipoProducto.ToString(),
            fresa.Calibre.Value, 
            fresa.Calidad,
            fresa.Peso.Value,    
            fresa.Descripcion
        );

        return Result
            .Success(response)
            .ToHttpCreatedAtResult($"/api/productos/fresa/{idGenerado}");
    }
}