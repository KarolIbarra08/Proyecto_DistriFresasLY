using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public class EliminarVentaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/ventas/{id:int}", Manejador)
           .WithName("EliminarVenta")
           .WithTags("Ventas")
           .WithSummary("Elimina una venta por su ID y devuelve el inventario");
    }

    private static async Task<IResult> Manejador(
        int id,
        IVentaRepository ventaRepository,
        IInventarioRepository inventarioRepository,
        CancellationToken cancellationToken)
    {
        var venta = await ventaRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (venta is null)
        {
            return Result.Failure(
                Error.NotFound("Venta.NotFound", $"No se encontró una venta con el ID: {id}"))
                .ToHttpResult();
        }

        if (await ventaRepository.TienePagosAsociadosAsync(id, cancellationToken))
        {
            return Result.Failure(
                Error.Conflict(
                    "Venta.PagosAsociados",
                    $"No se puede eliminar la venta con ID {id} porque tiene pagos asociados."))
                .ToHttpResult();
        }

        if (await ventaRepository.TieneReciboAsociadoAsync(id, cancellationToken))
        {
            return Result.Failure(
                Error.Conflict(
                    "Venta.ReciboAsociado",
                    $"No se puede eliminar la venta con ID {id} porque tiene un recibo asociado."))
                .ToHttpResult();
        }

        await ventaRepository.EliminarAsync(id, cancellationToken);

        var cantidades = venta.Detalles
            .GroupBy(d => d.ProductoId)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad.Value));

        foreach (var producto in cantidades)
        {
            var inventario = await inventarioRepository.ObtenerPorProductoIdAsync(producto.Key, cancellationToken);
            if (inventario is null) continue;

            var aumentarResult = inventario.Aumentar(producto.Value);
            if (aumentarResult.IsFailure) continue;

            await inventarioRepository.ActualizarAsync(inventario, cancellationToken);
        }

        var respuesta = new
        {
            mensaje = $"La venta con ID {id} se eliminó correctamente."
        };

        return Result.Success(respuesta).ToHttpResult();
    }
}
