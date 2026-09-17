namespace DistriFresasLY.Api.Contracts.Usuarios;

public record ActualizarUsuarioRequest(
    string? Nombre,
    string? Apellido,
    string? Usuario,
    string? Contrasena,
    string? Rol,
    bool Estado 
);