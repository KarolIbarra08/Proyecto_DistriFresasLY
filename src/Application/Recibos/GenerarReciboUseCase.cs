using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Recibos;

namespace DistriFresasLY.Application.Recibos;

public sealed record GenerarReciboCommand(int VentaId);

public sealed class GenerarReciboUseCase
{
    private readonly IReciboRepository _recibos;
    private readonly IVentaRepository _ventas;

    public GenerarReciboUseCase(
        IReciboRepository recibos,
        IVentaRepository ventas)
    {
        _recibos = recibos;
        _ventas = ventas;
    }

    public async Task<Result<ReciboDetalleDto>> EjecutarAsync(
        GenerarReciboCommand comando,
        CancellationToken cancellationToken = default)
    {
        var venta = await _ventas.ObtenerPorIdAsync(comando.VentaId, cancellationToken);
        if (venta is null)
        {
            return Result.Failure<ReciboDetalleDto>(
                ReciboErrores.VentaNoEncontrada(comando.VentaId));
        }

        var recibos = await _recibos.ObtenerTodosAsync(cancellationToken);

        var reciboExistente = recibos.FirstOrDefault(r => r.VentaId == comando.VentaId);
        if (reciboExistente is not null)
        {
            return Result.Failure<ReciboDetalleDto>(
                ReciboErrores.YaExiste(comando.VentaId, reciboExistente.NumeroRecibo));
        }

        var ultimoNumero = recibos.Count > 0 ? recibos.Max(r => r.NumeroRecibo) : 0;

        var reciboResult = Recibo.Create(comando.VentaId, venta.Total, ultimoNumero);
        if (reciboResult.IsFailure)
        {
            return Result.Failure<ReciboDetalleDto>(reciboResult.Error);
        }

        var nuevoId = await _recibos.AgregarAsync(reciboResult.Value, cancellationToken);

        var creado = await _recibos.ObtenerPorIdAsync(nuevoId, cancellationToken);
        if (creado is null)
        {
            return Result.Failure<ReciboDetalleDto>(ReciboErrores.ErrorCreacion);
        }

        return Result.Success(new ReciboDetalleDto(
            creado.Id,
            creado.FechaRecibo,
            creado.NumeroRecibo,
            creado.Total,
            creado.VentaId));
    }
}
