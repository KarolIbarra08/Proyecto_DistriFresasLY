namespace DistriFresasLY.Application.Roles;

public sealed record RolDetalle(
    int Id,
    string Nombre,
    string Descripcion,
    IReadOnlyList<string> Permisos);