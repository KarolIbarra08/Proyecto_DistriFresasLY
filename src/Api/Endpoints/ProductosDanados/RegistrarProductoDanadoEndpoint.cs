using DistriFresasLY.Api.Contracts.Descuentos;
using DistriFresasLY.Api.Contracts.ProductosDanados;
using DistriFresasLY.Api.Endpoints.Inventario;
using DistriFresasLY.Api.Endpoints.Productos;
using DistriFresasLY.Api.Extensions;
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
           .WithSummary("Registra un nuevo producto dañado y actualiza el inventario");
    }

    private static IResult Manejador(RegistrarProductoDanadoRequest request)
    {
        bool existeFresa = ProductoDataStore.FresasDb.Any(f => f.Id == request.ProductoId);
        bool existeInsumo = ProductoDataStore.InsumosDb.Any(i => i.Id == request.ProductoId);

        if (!existeFresa && !existeInsumo)
        {
            return Result.Failure<ProductoDanadoResponse>(Error.NotFound(
                "Producto.NotFound", $"No se encontró un producto con ID: {request.ProductoId}"))
                .ToHttpResult();
        }

        var nuevoId = ProductoDanadoDataStore.ProductosDanadosDb.Count != 0 
            ? ProductoDanadoDataStore.ProductosDanadosDb.Max(p => p.Id) + 1 
            : 1;

        // Si la fecha viene nula o vacía, usa la fecha/hora actual (DateTime.UtcNow)
        var fechaReal = request.FechaProductoDanado.HasValue && request.FechaProductoDanado.Value != default
            ? request.FechaProductoDanado.Value
            : DateTime.UtcNow;

        var result = ProductoDanado.Create(
            request.ProductoId,
            request.TipoProducto,
            request.Cantidad,
            request.Identificacion,
            request.Valor,
            fechaReal,
            request.Motivo,
            nuevoId);

        if (result.IsFailure)
            return Result.Failure<ProductoDanadoResponse>(result.Error).ToHttpResult();

        var entidad = result.Value;

        var (exito, _, mensajeError) = InventarioDataStore.DisminuirStock(entidad.ProductoId, entidad.Cantidad);

        if (!exito)
        {
            return Result.Failure<ProductoDanadoResponse>(
                Error.Validation("Inventario.StockInsuficiente", mensajeError ?? "Error al actualizar el stock.")
            ).ToHttpResult();
        }

        var response = MapearAResponse(entidad);
        ProductoDanadoDataStore.ProductosDanadosDb.Add(response);

        return Result.Success(response).ToHttpCreatedAtResult($"/api/productos/danados/{entidad.Id}");
    }

    private static ProductoDanadoResponse MapearAResponse(ProductoDanado p) =>
        new(
            p.Id,
            p.ProductoId,
            p.TipoProducto,
            p.Cantidad,
            p.Identificacion,
            p.Valor,
            p.FechaProductoDanado,
            p.Motivo,
            p.Descuento is null ? null : new DescuentoResponse(p.Descuento.Valor, p.Descuento.Fecha, p.Descuento.Motivo)
        );
}