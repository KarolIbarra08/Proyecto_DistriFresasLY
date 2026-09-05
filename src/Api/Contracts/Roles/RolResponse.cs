namespace DistriFresasLY.Api.Contracts.Roles;

public record RolResponse(
    int Id,
    string Nombre,
    string Descripcion,
    List<string> Permisos
);