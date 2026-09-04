namespace DistriFresasLY.Api.Contracts.Usuarios;

public record LoginRequest(
    string Usuario,
    string Contrasena
);