using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.DetalleRecepciones;

public sealed class EliminarDetalleRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;
    private readonly IInventarioRepository _inventarios;

    public EliminarDetalleRecepcionUseCase(
        IRecepcionRepository recepciones,
        IInventarioRepository inventarios)
    {
        _recepciones = recepciones;
        _inventarios = inventarios;
    }

    public async Task<Result> EjecutarAsync(int id, CancellationToken cancellationToken = default)
    {
        var recepciones = await _recepciones.ObtenerTodosAsync(cancellationToken);

        var recepcion = recepciones.FirstOrDefault(r => r.Detalles.Any(d => d.Id == id));
        if (recepcion is null)
        {
            return Result.Failure(DetalleRecepcionErrores.NoEncontrado(id));
        }

        var detalle = recepcion.Detalles.First(d => d.Id == id);
        var cantidad = (int)detalle.Cantidad;

        var inventario = await _inventarios.ObtenerPorProductoIdAsync(detalle.IdProducto, cancellationToken);
        if (inventario is not null)
        {
            var disminuirResult = inventario.Disminuir(cantidad);
            if (disminuirResult.IsFailure)
            {
                return Result.Failure(disminuirResult.Error);
            }

            await _inventarios.ActualizarAsync(inventario, cancellationToken);
        }

        var removerResult = recepcion.RemoverDetalle(detalle.IdProducto);
        if (removerResult.IsFailure)
        {
            return Result.Failure(removerResult.Error);
        }

        await _recepciones.ActualizarAsync(recepcion, cancellationToken);
        return Result.Success();
    }
}