using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
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

    private static IResult Manejador(
        CrearProductoFresaRequest request)
    {
        var nuevoId = ProductoDataStore.FresasDb.Count != 0
            ? ProductoDataStore.FresasDb.Max(p => p.Id) + 1
            : 1;

        var fechaReal =
            request.FechaIngreso.HasValue &&
            request.FechaIngreso.Value != default &&
            request.FechaIngreso.Value.Year > 1970
                ? request.FechaIngreso.Value
                : DateTime.UtcNow;

        var result = ProductoFresa.Create(
            request.Calibre,
            request.Calidad,
            request.Peso,
            request.PrecioCompra,
            request.Descripcion,
            nuevoId,
            fechaReal);

        if (result.IsFailure)
        {
            return Result
                .Failure<ProductoFresaResponse>(result.Error)
                .ToHttpResult();
        }

        var fresa = result.Value;

        ProductoDataStore.FresasDb.Add(fresa);

        var response = new ProductoFresaResponse(
            fresa.Id,
            fresa.TipoProducto,
            fresa.Calibre.Value,
            fresa.Calidad,
            fresa.Peso.Value,
            fresa.PrecioCompra.Value,
            fresa.FechaIngreso,
            fresa.Descripcion
        );

        return Result
            .Success(response)
            .ToHttpCreatedAtResult(
                $"/api/productos/fresa/{fresa.Id}");
    }
}