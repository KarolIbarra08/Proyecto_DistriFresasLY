using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Recepciones;

public sealed class EliminarRecepcionUseCase
{
    private readonly IRecepcionRepository _recepciones;

    public EliminarRecepcionUseCase(IRecepcionRepository recepciones) => _recepciones = recepciones;

    public async Task<Result> EjecutarAsync(int id, CancellationToken cancellationToken = default)
    {
        var recepcion = await _recepciones.ObtenerPorIdAsync(id, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure(RecepcionErrores.NoEncontrada(id));
        }

        await _recepciones.EliminarAsync(id, cancellationToken);
        return Result.Success();
    }
}