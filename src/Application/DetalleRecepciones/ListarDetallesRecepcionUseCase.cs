using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Application.Recepciones;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.DetalleRecepciones;

public sealed class ListarDetallesRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;

    public ListarDetallesRecepcionUseCase(IRecepcionRepository recepciones) => _recepciones = recepciones;

    public async Task<Result<IReadOnlyList<DetalleRecepcionItemDto>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var recepciones = await _recepciones.ObtenerTodosAsync(cancellationToken);

        var detalles = recepciones
            .SelectMany(r => r.Detalles)
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