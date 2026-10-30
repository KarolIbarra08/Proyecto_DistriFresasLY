using DistriFresasLY.Domain.Entities.Roles;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

public record RolRegistro
{
    public int Id_Rol { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }

    public Rol ToEntidad(List<string>? permisos = null)
    {
        var result = Rol.Create(
            nombre: Nombre,
            descripcion: Descripcion,
            permisos: permisos,
            id: Id_Rol);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Error al reconstruir la entidad Rol ({Id_Rol}): {result.Error.Description}");
        }

        return result.Value;
    }
}