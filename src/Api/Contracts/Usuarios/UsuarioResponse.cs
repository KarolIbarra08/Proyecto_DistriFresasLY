namespace DistriFresasLY.Api.Contracts.Usuarios;

public record UsuarioResponse(
    int Id,
    string Nombre,
    string Apellido,
    string Usuario,
    string Contrasena,
    string Rol,
    bool Estado 
);
