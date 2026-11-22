using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Recepciones;

public sealed class ObtenerDetallesRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;

    public ObtenerDetallesRecepcionUseCase(IRecepcionRepository recepciones) => _recepciones = recepciones;

    public async Task<Result<IReadOnlyList<DetalleRecepcionItemDto>>> EjecutarAsync(
        int idRecepcion,
        CancellationToken cancellationToken = default)
    {
        var recepcion = await _recepciones.ObtenerPorIdAsync(idRecepcion, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<IReadOnlyList<DetalleRecepcionItemDto>>(
                RecepcionErrores.NoEncontrada(idRecepcion));
        }

        var detalles = recepcion.Detalles
            .Select(d => new DetalleRecepcionItemDto(
                d.Id,
                d.IdRecepcion,
                d.IdProducto,
                (int)d.Cantidad,
                d.PrecioUnitario,
                d.Subtotal,
                d.FechaIngreso))
            .ToList();

        return Result.Success<IReadOnlyList<DetalleRecepcionItemDto>>(detalles);
    }
}