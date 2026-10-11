using DistriFresasLY.Domain.Entities.Usuarios;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public static class UsuarioDataStore
{
    public static readonly List<Usuario> UsuariosDb =
    [
        Usuario.Create(
            nombre: "Admin",
            apellido: "Sistema",
            usuario: "admin",
            contrasena: "123456",
            rol: "Administrador",
            id: 1).Value
    ];
}