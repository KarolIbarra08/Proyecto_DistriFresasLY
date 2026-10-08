using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Endpoints.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumo;

public class RegistrarProductoInsumoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/productos/insumo", Manejador)
           .WithName("RegistrarProductoInsumo")
           .WithTags("Productos Insumo")
           .WithSummary("Registra un nuevo producto insumo e inicializa su stock");
    }

    private static IResult Manejador(CrearProductoInsumoRequest request)
    {
        var nuevoId = ProductoDataStore.InsumosDb.Count != 0 
            ? ProductoDataStore.InsumosDb.Max(p => p.Id) + 1 
            : 101;

        var fechaReal = request.FechaIngreso.HasValue 
            && request.FechaIngreso.Value != default 
            && request.FechaIngreso.Value.Year > 1970
                ? request.FechaIngreso.Value
                : DateTime.UtcNow;

        var result = ProductoInsumo.Create(
            request.Nombre,
            request.Tipo,
            request.UnidadMedida,
            request.Descripcion,
            nuevoId,
            fechaReal);

        if (result.IsFailure)
            return Result.Failure<ProductoInsumoResponse>(result.Error).ToHttpResult();

        var insumo = result.Value;
        ProductoDataStore.InsumosDb.Add(insumo);

        InventarioDataStore.InicializarStock(insumo.Id, request.CantidadInicial);

        var response = new ProductoInsumoResponse(
            insumo.Id,
            insumo.TipoProducto,
            insumo.Nombre,
            insumo.Tipo,
            insumo.UnidadMedida,
            insumo.Descripcion,
            insumo.FechaIngreso,
            request.CantidadInicial
        );

        return Result.Success(response).ToHttpCreatedAtResult($"/api/productos/insumo/{insumo.Id}");
    }
}