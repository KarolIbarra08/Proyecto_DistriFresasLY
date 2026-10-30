using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Fresas;

public class ActualizarProductoFresaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/productos/fresa/{id:int}", Manejador)
           .WithName("ActualizarProductoFresa")
           .WithTags("Productos Fresa")
           .WithSummary("Actualiza un producto fresa existente");
    }

    private static async Task<IResult> Manejador(
        int id,
        ActualizarProductoFresaRequest request,
        IProductoRepository productoRepository,
        CancellationToken cancellationToken)
    {

        var productoExistente = await productoRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (productoExistente is not ProductoFresa fresa)
        {
            return Result
                .Failure<ProductoFresaResponse>(
                    Error.NotFound(
                        "ProductoFresa.NotFound",
                        $"No se encontró el producto fresa con ID: {id}"))
                .ToHttpResult();
        }

      
        int calibreFinal = request.Calibre > 0 ? request.Calibre : fresa.Calibre.Value;
        string calidadFinal = !string.IsNullOrWhiteSpace(request.Calidad) ? request.Calidad : fresa.Calidad;
        double pesoFinal = request.Peso > 0 ? request.Peso : fresa.Peso.Value;
        string descripcionFinal = !string.IsNullOrWhiteSpace(request.Descripcion) ? request.Descripcion : fresa.Descripcion;


        bool ExisteCalibreParaOtro(int c) => productoRepository
            .ExisteCalibreAsync(c, idExcluir: id, cancellationToken)
            .GetAwaiter()
            .GetResult();

 
        var updateResult = fresa.Actualizar(
            calibreFinal,
            calidadFinal,
            pesoFinal,
            descripcionFinal,
            ExisteCalibreParaOtro);

        if (updateResult.IsFailure)
        {
            return Result
                .Failure<ProductoFresaResponse>(updateResult.Error)
                .ToHttpResult();
        }


        await productoRepository.ActualizarFresaAsync(fresa, cancellationToken);


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