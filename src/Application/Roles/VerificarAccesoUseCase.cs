using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Roles;

public sealed record VerificarAccesoQuery(
    int IdRol,
    string Permiso);

public sealed class VerificarAccesoUseCase
{
    private readonly IRolRepository _roles;

    public VerificarAccesoUseCase(IRolRepository roles)
    {
        _roles = roles;
    }

    public async Task<Result<bool>> EjecutarAsync(
        VerificarAccesoQuery consulta,
        CancellationToken cancellationToken = default)
    {
        var rol = await _roles.ObtenerPorIdAsync(consulta.IdRol, cancellationToken);
        if (rol is null)
        {
            return Result.Failure<bool>(RolErrores.NoEncontrado(consulta.IdRol));
        }

        if (!rol.TieneAcceso(consulta.Permiso))
        {
            return Result.Failure<bool>(RolErrores.AccesoDenegado(rol.Nombre, consulta.Permiso));
        }

        return Result.Success(true);
    }
}