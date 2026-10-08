
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
           .WithSummary("Registra un producto dañado y disminuye el inventario");
    }

    private static IResult Manejador(
        RegistrarProductoDanadoRequest request)
    {

        var existeFresa = ProductoDataStore.FresasDb
            .Any(f => f.Id == request.ProductoId);

        var existeInsumo = ProductoDataStore.InsumosDb
            .Any(i => i.Id == request.ProductoId);

        if (!existeFresa && !existeInsumo)
        {
            return Result.Failure<ProductoDanadoResponse>(
                Error.NotFound(
                    "Producto.NotFound",
                    $"No se encontró un producto con ID: {request.ProductoId}"))
                .ToHttpResult();
        }

 
        var nuevoId = ProductoDanadoDataStore.ProductosDanadosDb.Count != 0
            ? ProductoDanadoDataStore.ProductosDanadosDb.Max(p => p.Id) + 1
            : 1;


        var fechaReal =
            request.FechaProductoDanado.HasValue &&
            request.FechaProductoDanado.Value != default
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
        {
            return Result.Failure<ProductoDanadoResponse>(
                result.Error)
                .ToHttpResult();
        }

        var entidad = result.Value;


        var (exito, inventario, mensajeError) =
            InventarioDataStore.DisminuirStock(
                entidad.ProductoId,
                entidad.Cantidad);

        if (!exito)
        {
            var error = inventario is null
                ? Error.NotFound(
                    "Inventario.NotFound",
                    mensajeError ?? "No existe inventario para el producto.")
                : Error.Validation(
                    "Inventario.StockInsuficiente",
                    mensajeError ?? "El stock es insuficiente.");

            return Result.Failure<ProductoDanadoResponse>(
                error)
                .ToHttpResult();
        }


        var response = new ProductoDanadoResponse(
            entidad.Id,
            entidad.ProductoId,
            entidad.TipoProducto,
            entidad.Cantidad,
            entidad.Identificacion,
            entidad.Valor,
            entidad.FechaProductoDanado,
            entidad.Motivo);

        ProductoDanadoDataStore.ProductosDanadosDb.Add(response);

        return Result.Success(response)
            .ToHttpCreatedAtResult(
                $"/api/productos/danados/{entidad.Id}");
    }
}

