using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumos;

public class RegistrarProductoInsumoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/productos/insumo", Manejador)
           .WithName("RegistrarProductoInsumo")
           .WithTags("Productos Insumo")
           .WithSummary("Registra un nuevo producto insumo");
    }

    private static IResult Manejador(
        CrearProductoInsumoRequest request)
    {
        var nuevoId = ProductoDataStore.InsumosDb.Count != 0
            ? ProductoDataStore.InsumosDb.Max(p => p.Id) + 1
            : 101;

        var fechaReal =
            request.FechaIngreso.HasValue &&
            request.FechaIngreso.Value != default &&
            request.FechaIngreso.Value.Year > 1970
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
        {
            return Result
                .Failure<ProductoInsumoResponse>(result.Error)
                .ToHttpResult();
        }

        var insumo = result.Value;

        ProductoDataStore.InsumosDb.Add(insumo);

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
            .ToHttpCreatedAtResult(
                $"/api/productos/insumo/{insumo.Id}");
    }
}