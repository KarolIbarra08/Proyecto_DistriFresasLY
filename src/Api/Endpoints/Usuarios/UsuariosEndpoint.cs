using DistriFresasLY.Api.Contracts.Usuarios;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public record UsuarioModel(
    int Id,
    string Nombre,
    string Apellido,
    string Usuario,
    string Contrasena,
    string Rol,
    bool Estado
);

public static class UsuarioDataStore
{
    public static readonly List<UsuarioModel> UsuariosDb =
    [
        new(
            Id: 1,
            Nombre: "Admin",
            Apellido: "Sistema",
            Usuario: "admin",
            Contrasena: "123456",
            Rol: "Administrador",
            Estado: true
        )
    ];
}