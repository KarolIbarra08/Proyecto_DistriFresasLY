using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Usuarios;

public sealed class EliminarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarios;

    public EliminarUsuarioUseCase(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<Result> EjecutarAsync(int id, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(id, cancellationToken);
        if (usuario is null)
        {
            return Result.Failure(UsuarioErrores.NoEncontrado(id));
        }

        await _usuarios.EliminarAsync(id, cancellationToken);
        return Result.Success();
    }
}