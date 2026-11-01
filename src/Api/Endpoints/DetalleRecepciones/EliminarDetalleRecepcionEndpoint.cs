using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.DetalleRecepciones;

public class EliminarDetalleRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/detalles-recepcion/{id:int}", Manejador)
           .WithName("EliminarDetalleRecepcion")
           .WithTags("DetalleRecepciones")
           .WithSummary("Elimina un detalle y actualiza el inventario");
    }

    private static async Task<IResult> Manejador(
        int id,
        IRecepcionRepository recepcionRepository,
        IInventarioRepository inventarioRepository,
        CancellationToken cancellationToken)
    {
        var recepciones = await recepcionRepository.ObtenerTodosAsync(cancellationToken);

        var recepcion = recepciones.FirstOrDefault(r => r.Detalles.Any(d => d.Id == id));
        if (recepcion is null)
        {
            return Result.Failure(
                Error.NotFound("DetalleRecepcion.NotFound",
                    $"No se encontró el detalle con ID: {id}"))
                .ToHttpResult();
        }

        var detalle = recepcion.Detalles.First(d => d.Id == id);
        var cantidad = (int)detalle.Cantidad;

        var inventario = await inventarioRepository.ObtenerPorProductoIdAsync(detalle.IdProducto, cancellationToken);
        if (inventario is not null)
        {
            var disminuirResult = inventario.Disminuir(cantidad);
            if (disminuirResult.IsFailure)
            {
                return Result.Failure(disminuirResult.Error).ToHttpResult();
            }

            await inventarioRepository.ActualizarAsync(inventario, cancellationToken);
        }

        var removerResult = recepcion.RemoverDetalle(detalle.IdProducto);
        if (removerResult.IsFailure)
        {
            return Result.Failure(removerResult.Error).ToHttpResult();
        }

        await recepcionRepository.ActualizarAsync(recepcion, cancellationToken);

        return Result.Success().ToHttpResult();
    }
}
