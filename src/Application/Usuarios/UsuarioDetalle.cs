namespace DistriFresasLY.Application.Usuarios;

public sealed record UsuarioDetalle(
    int Id,
    string Nombre,
    string Apellido,
    string Usuario,
    string Contrasena,
    string Rol,
    bool Estado);