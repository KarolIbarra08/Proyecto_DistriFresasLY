using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Roles;

public sealed record ActualizarRolCommand(
    int Id,
    string? Nombre,
    string? Descripcion);

public sealed class ActualizarRolUseCase
{
    private readonly IRolRepository _roles;

    public ActualizarRolUseCase(IRolRepository roles)
    {
        _roles = roles;
    }

    public async Task<Result<RolDetalle>> EjecutarAsync(
        ActualizarRolCommand comando,
        CancellationToken cancellationToken = default)
    {
        var rol = await _roles.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (rol is null)
        {
            return Result.Failure<RolDetalle>(RolErrores.NoEncontrado(comando.Id));
        }

        var nombre = !string.IsNullOrWhiteSpace(comando.Nombre) ? comando.Nombre.Trim() : rol.Nombre;
        var descripcion = comando.Descripcion is not null ? comando.Descripcion.Trim() : rol.Descripcion;

        if (!string.Equals(nombre, rol.Nombre, StringComparison.OrdinalIgnoreCase))
        {
            var rolDuplicado = await _roles.ObtenerPorNombreAsync(nombre, cancellationToken);
            if (rolDuplicado is not null && rolDuplicado.Id != comando.Id)
            {
                return Result.Failure<RolDetalle>(RolErrores.Duplicado(nombre));
            }
        }

        var actualizarResult = rol.Actualizar(nombre, descripcion);
        if (actualizarResult.IsFailure)
        {
            return Result.Failure<RolDetalle>(actualizarResult.Error);
        }

        await _roles.ActualizarAsync(rol, cancellationToken);

        return Result.Success(new RolDetalle(
            rol.Id,
            rol.Nombre,
            rol.Descripcion,
            rol.Permisos.ToList()));
    }
}