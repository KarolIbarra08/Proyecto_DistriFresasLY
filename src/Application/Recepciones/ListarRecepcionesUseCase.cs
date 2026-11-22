using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Recepciones;

public sealed class ListarRecepcionesUseCase
{
    private readonly IRecepcionRepository _recepciones;

    public ListarRecepcionesUseCase(IRecepcionRepository recepciones) => _recepciones = recepciones;

    public async Task<Result<IReadOnlyList<RecepcionDetalleDto>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var recepciones = await _recepciones.ObtenerTodosAsync(cancellationToken);

        var lista = recepciones.Select(r => new RecepcionDetalleDto(
            r.Id,
            r.PedidoId,
            r.FechaRecepcion,
            r.ValorPago,
            r.ProcesadaEnInventario,
            r.CalcularTotal())).ToList();

        return Result.Success<IReadOnlyList<RecepcionDetalleDto>>(lista);
    }
}