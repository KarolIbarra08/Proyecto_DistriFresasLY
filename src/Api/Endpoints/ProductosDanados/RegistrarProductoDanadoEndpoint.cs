using DistriFresasLY.Api.Contracts.ProductosDanados;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.ProductosDanados;

namespace DistriFresasLY.Api.Endpoints.ProductosDanados;

public class RegistrarProductoDanadoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/productos/danados", Manejador)
           .WithName("RegistrarProductoDanado")
           .WithTags("Productos Dañados")
           .WithSummary("Registra un producto dañado en la base de datos y descuenta del inventario");
    }

    private static async Task<IResult> Manejador(
        RegistrarProductoDanadoRequest request,
        IProductoRepository productoRepository,
        IProductoDanadoRepository productoDanadoRepository,
        IInventarioRepository inventarioRepository,
        CancellationToken cancellationToken)
    {

        if (request.ProductoId <= 0 || request.Cantidad <= 0)
        {
            return Result.Failure<ProductoDanadoResponse>(
                Error.Validation("ProductoDanado.DatosInvalidos", "El ProductoId y la Cantidad deben ser mayores a 0."))
                .ToHttpResult();
        }

        var producto = await productoRepository.ObtenerPorIdAsync(request.ProductoId, cancellationToken);
        if (producto is null)
        {
            return Result.Failure<ProductoDanadoResponse>(
                Error.NotFound("Producto.NotFound", $"No se encontró un producto con ID: {request.ProductoId}."))
                .ToHttpResult();
        }


        var inventario = await inventarioRepository.ObtenerPorProductoIdAsync(request.ProductoId, cancellationToken);
        if (inventario is null)
        {
            return Result.Failure<ProductoDanadoResponse>(
                Error.NotFound("Inventario.NotFound", $"No se encontró registro de inventario para el producto con ID: {request.ProductoId}."))
                .ToHttpResult();
        }

        var disminuirResult = inventario.Disminuir(request.Cantidad);
        if (disminuirResult.IsFailure)
        {
            return Result.Failure<ProductoDanadoResponse>(disminuirResult.Error).ToHttpResult();
        }

        var fechaReal = request.FechaProductoDanado.HasValue &&
                        request.FechaProductoDanado.Value != default &&
                        request.FechaProductoDanado.Value.Year > 1970
            ? request.FechaProductoDanado.Value
            : ColombiaTime.Ahora;

  
        var result = ProductoDanado.Create(
            request.ProductoId,
            request.TipoProducto,
            request.Cantidad,
            request.Identificacion,
            request.Valor,
            fechaReal,
            request.Motivo);

        if (result.IsFailure)
        {
            return Result.Failure<ProductoDanadoResponse>(result.Error).ToHttpResult();
        }

       
        await inventarioRepository.ActualizarAsync(inventario, cancellationToken);

        var entidad = result.Value;
        int nuevoId = await productoDanadoRepository.AgregarAsync(entidad, cancellationToken);

        var response = new ProductoDanadoResponse(
            nuevoId,
            entidad.ProductoId,
            entidad.TipoProducto,
            entidad.Cantidad,
            entidad.Identificacion,
            (double)entidad.Valor,
            entidad.FechaProductoDanado,
            entidad.Motivo);

        return Result.Success(response)
            .ToHttpCreatedAtResult($"/api/productos/danados/{nuevoId}");
    }
}