using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Endpoints.Inventario;
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
           .WithSummary("Registra un nuevo producto fresa e inicializa su stock");
    }

    private static IResult Manejador(CrearProductoFresaRequest request)
    {
        var nuevoId = ProductoDataStore.FresasDb.Count != 0 
            ? ProductoDataStore.FresasDb.Max(p => p.Id) + 1 
            : 1;

        var result = ProductoFresa.Create(
            request.Calibre,
            request.Calidad,
            request.Peso,
            request.PrecioCompra,
            request.Descripcion,
            nuevoId);

        if (result.IsFailure)
            return Result.Failure<ProductoFresaResponse>(result.Error).ToHttpResult();

        var fresa = result.Value;
        ProductoDataStore.FresasDb.Add(fresa);

        // Inicializa el inventario
        InventarioDataStore.AjustarStock(fresa.Id, request.CantidadInicial);

        // Se envía request.CantidadInicial al final del constructor
        var response = new ProductoFresaResponse(
            fresa.Id,
            fresa.TipoProducto,
            fresa.Calibre,
            fresa.Calidad,
            fresa.Peso,
            fresa.PrecioCompra,
            fresa.FechaIngreso,
            fresa.Descripcion,
            request.CantidadInicial 
        );

        return Result.Success(response).ToHttpCreatedAtResult($"/api/productos/fresa/{fresa.Id}");
    }
}