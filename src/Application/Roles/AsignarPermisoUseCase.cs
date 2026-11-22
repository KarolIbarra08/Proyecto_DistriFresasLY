using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Roles;

public sealed record AsignarPermisoCommand(
    int IdRol,
    string Permiso);

public sealed class AsignarPermisoUseCase
{
    private readonly IRolRepository _roles;

    public AsignarPermisoUseCase(IRolRepository roles)
    {
        _roles = roles;
    }

    public async Task<Result<RolDetalle>> EjecutarAsync(
        AsignarPermisoCommand comando,
        CancellationToken cancellationToken = default)
    {
        var rol = await _roles.ObtenerPorIdAsync(comando.IdRol, cancellationToken);
        if (rol is null)
        {
            return Result.Failure<RolDetalle>(RolErrores.NoEncontrado(comando.IdRol));
        }

        var asignarResult = rol.AgregarPermiso(comando.Permiso);
        if (asignarResult.IsFailure)
        {
            return Result.Failure<RolDetalle>(asignarResult.Error);
        }

        await _roles.ActualizarAsync(rol, cancellationToken);

        return Result.Success(new RolDetalle(
            rol.Id,
            rol.Nombre,
            rol.Descripcion,
            rol.Permisos.ToList()));
    }
}