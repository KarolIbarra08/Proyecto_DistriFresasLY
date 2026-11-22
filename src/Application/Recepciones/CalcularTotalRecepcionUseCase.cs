using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Recepciones;

public sealed class CalcularTotalRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;

    public CalcularTotalRecepcionUseCase(IRecepcionRepository recepciones) => _recepciones = recepciones;

    public async Task<Result<double>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var recepcion = await _recepciones.ObtenerPorIdAsync(id, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<double>(RecepcionErrores.NoEncontrada(id));
        }

        return Result.Success(recepcion.CalcularTotal());
    }
}