using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Endpoints.Inventario;
using DistriFresasLY.Api.Endpoints.Productos;
using DistriFresasLY.Api.Endpoints.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Recepciones;

namespace DistriFresasLY.Api.Endpoints.DetalleRecepciones;

public class RegistrarDetalleRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/detalles-recepcion", Manejador)
           .WithName("RegistrarDetalleRecepcion")
           .WithTags("DetalleRecepciones")
           .WithSummary("Registra un detalle asociado a una recepción y actualiza el inventario");
    }

    private static IResult Manejador(
        RegistrarDetalleRecepcionRequest request)
    {
        var recepcion = RecepcionDataStore.RecepcionesDb
            .FirstOrDefault(r => r.Id == request.IdRecepcion);

        if (recepcion is null)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.NotFound(
                    "Recepcion.NotFound",
                    $"No se encontró la recepción con ID: {request.IdRecepcion}."))
                .ToHttpResult();
        }

        var productoFresaExiste =
            ProductoDataStore.FresasDb
                .Any(p => p.Id == request.IdProducto);

        var productoInsumoExiste =
            ProductoDataStore.InsumosDb
                .Any(p => p.Id == request.IdProducto);

        if (!productoFresaExiste && !productoInsumoExiste)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.NotFound(
                    "Producto.NotFound",
                    $"No se encontró el producto con ID: {request.IdProducto}."))
                .ToHttpResult();
        }

        var nuevoId =
            DetalleRecepcionDataStore.DetallesDb.Count != 0
                ? DetalleRecepcionDataStore.DetallesDb.Max(d => d.Id) + 1
                : 1;

        var result = DetalleRecepcion.Create(
            request.IdProducto,
            request.Cantidad,
            request.PrecioUnitario,
            request.IdRecepcion,
            id: nuevoId,
            fechaIngreso: request.FechaIngreso
        );

        if (result.IsFailure)
        {
            return Result.Failure<DetalleRecepcionResponse>(result.Error)
                .ToHttpResult();
        }

        var detalle = result.Value;

       
        var agregarResult = recepcion.AgregarDetalle(detalle);
        if (agregarResult.IsFailure)
        {
            return Result.Failure<DetalleRecepcionResponse>(agregarResult.Error)
                .ToHttpResult();
        }

     
        DetalleRecepcionDataStore.DetallesDb.Add(detalle);

     
        var resultadoInventario = InventarioDataStore.AumentarStock(
            detalle.IdProducto,
            (int)detalle.Cantidad
        );

        if (resultadoInventario is null)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.Validation(
                    "Inventario.Error",
                    "No fue posible encontrar el registro de inventario para el producto especificado."))
                .ToHttpResult();
        }

        var response = new DetalleRecepcionResponse(
            detalle.Id,
            detalle.IdRecepcion,
            detalle.IdProducto,
            detalle.Cantidad,
            detalle.PrecioUnitario,
            detalle.Subtotal,
            detalle.FechaIngreso
        );

        return Result.Success(response)
            .ToHttpCreatedAtResult(
                $"/api/detalles-recepcion/{detalle.Id}");
    }
}