using DistriFresasLY.Domain.Entities.Usuarios;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

public class UsuarioRegistro
{
    public int Id_Usuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Nombre_Usuario { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Estado { get; set; }

    public Usuario ToEntidad()
    {
        var result = Usuario.Create(
            nombre: Nombre,
            apellido: Apellido,
            usuario: Nombre_Usuario,
            contrasena: Contrasena,
            rol: Rol,
            estado: Estado,
            id: Id_Usuario);

        return result.Value;
    }
}