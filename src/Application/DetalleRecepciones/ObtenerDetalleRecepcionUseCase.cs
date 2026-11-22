using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Application.Recepciones;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.DetalleRecepciones;

public sealed class ObtenerDetalleRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;

    public ObtenerDetalleRecepcionUseCase(IRecepcionRepository recepciones) => _recepciones = recepciones;

    public async Task<Result<DetalleRecepcionItemDto>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var recepciones = await _recepciones.ObtenerTodosAsync(cancellationToken);

        var detalle = recepciones
            .SelectMany(r => r.Detalles)
            .FirstOrDefault(d => d.Id == id);

        if (detalle is null)
        {
            return Result.Failure<DetalleRecepcionItemDto>(DetalleRecepcionErrores.NoEncontrado(id));
        }

        return Result.Success(new DetalleRecepcionItemDto(
            detalle.Id,
            detalle.IdRecepcion,
            detalle.IdProducto,
            (int)detalle.Cantidad,
            detalle.PrecioUnitario,
            detalle.Subtotal,
            detalle.FechaIngreso));
    }
}