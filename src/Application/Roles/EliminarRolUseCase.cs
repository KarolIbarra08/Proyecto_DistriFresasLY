using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Roles;

public sealed class EliminarRolUseCase
{
    private readonly IRolRepository _roles;

    public EliminarRolUseCase(IRolRepository roles)
    {
        _roles = roles;
    }

    public async Task<Result> EjecutarAsync(int id, CancellationToken cancellationToken = default)
    {
        var rol = await _roles.ObtenerPorIdAsync(id, cancellationToken);
        if (rol is null)
        {
            return Result.Failure(RolErrores.NoEncontrado(id));
        }

        var tieneUsuarios = await _roles.TieneUsuariosAsociadosAsync(id, cancellationToken);
        if (tieneUsuarios)
        {
            return Result.Failure(RolErrores.TieneUsuariosAsociados(id));
        }

        await _roles.EliminarAsync(id, cancellationToken);
        return Result.Success();
    }
}