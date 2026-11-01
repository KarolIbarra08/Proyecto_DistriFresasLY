using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Recepciones;
using InventarioEntity = DistriFresasLY.Domain.Entities.Inventario.Inventario;

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

    private static async Task<IResult> Manejador(
        RegistrarDetalleRecepcionRequest request,
        IRecepcionRepository recepcionRepository,
        IProductoRepository productoRepository,
        IInventarioRepository inventarioRepository,
        CancellationToken cancellationToken)
    {
        
        if (request.IdRecepcion <= 0 || request.IdProducto <= 0)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.Validation("DetalleRecepcion.IdsInvalidos", "El IdRecepcion y el IdProducto deben ser mayores a 0."))
                .ToHttpResult();
        }

        if (request.Cantidad <= 0)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.Validation("DetalleRecepcion.CantidadInvalida", "La cantidad debe ser mayor a 0."))
                .ToHttpResult();
        }

        if (request.PrecioUnitario <= 0)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.Validation("DetalleRecepcion.PrecioInvalido", "El precio unitario debe ser mayor a 0."))
                .ToHttpResult();
        }

        var recepcion = await recepcionRepository.ObtenerPorIdAsync(request.IdRecepcion, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.NotFound("Recepcion.NotFound",
                    $"No se encontró la recepción con ID: {request.IdRecepcion}."))
                .ToHttpResult();
        }


        var producto = await productoRepository.ObtenerPorIdAsync(request.IdProducto, cancellationToken);
        if (producto is null)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.NotFound("Producto.NotFound",
                    $"No se encontró el producto con ID: {request.IdProducto}."))
                .ToHttpResult();
        }


        var detalleResult = DetalleRecepcion.Create(
            idProducto: request.IdProducto,
            cantidad: request.Cantidad,
            precioUnitario: request.PrecioUnitario,
            idRecepcion: request.IdRecepcion,
            fechaIngreso: request.FechaIngreso);

        if (detalleResult.IsFailure)
        {
            return Result.Failure<DetalleRecepcionResponse>(detalleResult.Error).ToHttpResult();
        }

        var agregarResult = recepcion.AgregarDetalle(detalleResult.Value);
        if (agregarResult.IsFailure)
        {
            return Result.Failure<DetalleRecepcionResponse>(agregarResult.Error).ToHttpResult();
        }

        await recepcionRepository.ActualizarAsync(recepcion, cancellationToken);


        var inventario = await inventarioRepository.ObtenerPorProductoIdAsync(request.IdProducto, cancellationToken);

        if (inventario is null)
        {
            var nuevoInventario = InventarioEntity.Create(request.IdProducto, request.Cantidad);
            if (nuevoInventario.IsFailure)
            {
                return Result.Failure<DetalleRecepcionResponse>(nuevoInventario.Error).ToHttpResult();
            }

            await inventarioRepository.AgregarAsync(nuevoInventario.Value, cancellationToken);
        }
        else
        {
            var aumentarResult = inventario.Aumentar(request.Cantidad);
            if (aumentarResult.IsFailure)
            {
                return Result.Failure<DetalleRecepcionResponse>(aumentarResult.Error).ToHttpResult();
            }

            await inventarioRepository.ActualizarAsync(inventario, cancellationToken);
        }

        // 6. Obtener detalle creado y mapear respuesta
        var guardada = await recepcionRepository.ObtenerPorIdAsync(request.IdRecepcion, cancellationToken);
        var detalle = guardada!.Detalles
            .LastOrDefault(d => d.IdProducto == request.IdProducto);

        if (detalle is null)
        {
            return Result.Failure<DetalleRecepcionResponse>(
                Error.Failure("DetalleRecepcion.ErrorCreacion", "No se pudo recuperar el detalle creado."))
                .ToHttpResult();
        }

        var response = new DetalleRecepcionResponse(
            detalle.Id,
            detalle.IdRecepcion,
            detalle.IdProducto,
            (int)detalle.Cantidad,
            detalle.PrecioUnitario,
            detalle.Subtotal,
            detalle.FechaIngreso);

        return Result.Success(response).ToHttpCreatedAtResult($"/api/detalles-recepcion/{detalle.Id}");
    }
}