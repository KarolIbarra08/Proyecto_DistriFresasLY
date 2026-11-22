using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Roles;

public sealed class ObtenerRolesUseCase
{
    private readonly IRolRepository _roles;

    public ObtenerRolesUseCase(IRolRepository roles)
    {
        _roles = roles;
    }

    public async Task<Result<IReadOnlyList<RolDetalle>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var roles = await _roles.ObtenerTodosAsync(cancellationToken);

        var detalles = roles.Select(r => new RolDetalle(
            r.Id,
            r.Nombre,
            r.Descripcion,
            r.Permisos.ToList())).ToList();

        return Result.Success<IReadOnlyList<RolDetalle>>(detalles);
    }
}