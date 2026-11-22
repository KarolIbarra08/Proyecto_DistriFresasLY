using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Roles;

namespace DistriFresasLY.Application.Roles;

public sealed record CrearRolCommand(
    string Nombre,
    string? Descripcion);

public sealed class CrearRolUseCase
{
    private readonly IRolRepository _roles;

    public CrearRolUseCase(IRolRepository roles)
    {
        _roles = roles;
    }

    public async Task<Result<RolDetalle>> EjecutarAsync(
        CrearRolCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(comando.Nombre))
        {
            var rolExistente = await _roles.ObtenerPorNombreAsync(comando.Nombre, cancellationToken);
            if (rolExistente is not null)
            {
                return Result.Failure<RolDetalle>(RolErrores.Duplicado(comando.Nombre));
            }
        }

        var rolResult = Rol.Create(
            nombre: comando.Nombre,
            descripcion: comando.Descripcion,
            id: 0);

        if (rolResult.IsFailure)
        {
            return Result.Failure<RolDetalle>(rolResult.Error);
        }

        var rol = rolResult.Value;
        var nuevoId = await _roles.AgregarAsync(rol, cancellationToken);

        return Result.Success(new RolDetalle(
            nuevoId,
            rol.Nombre,
            rol.Descripcion,
            rol.Permisos.ToList()));
    }
}