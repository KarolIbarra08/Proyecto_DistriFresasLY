using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Recepciones;

public sealed class ObtenerRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;
    private readonly IProductoDanadoRepository _danados;

    public ObtenerRecepcionUseCase(
        IRecepcionRepository recepciones,
        IProductoDanadoRepository danados)
    {
        _recepciones = recepciones;
        _danados = danados;
    }

    public async Task<Result<RecepcionDetalleDto>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var recepcion = await _recepciones.ObtenerPorIdAsync(id, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<RecepcionDetalleDto>(RecepcionErrores.NoEncontrada(id));
        }

        var productosDanados = await _danados.ObtenerTodosAsync(cancellationToken);

        double totalDescuentos = (double)productosDanados
            .Where(pd => pd.Descuento is not null &&
                         recepcion.Detalles.Any(d => d.IdProducto == pd.ProductoId))
            .Sum(pd => pd.Descuento!.Valor);

        double totalCalculadoOriginal = recepcion.CalcularTotal();
        double valorPagoFinal = Math.Max(0, totalCalculadoOriginal - totalDescuentos);

        return Result.Success(new RecepcionDetalleDto(
            recepcion.Id,
            recepcion.PedidoId,
            recepcion.FechaRecepcion,
            valorPagoFinal,
            recepcion.ProcesadaEnInventario,
            totalCalculadoOriginal));
    }
}