using DistriFresasLY.Application.Usuarios;

namespace DistriFresasLY.Api.Contracts.Usuarios;

public static class UsuarioMappingExtensions
{
    public static CrearUsuarioCommand ToCommand(this CrearUsuarioRequest request) => new(
        request.Nombre,
        request.Apellido,
        request.Usuario,
        request.Contrasena,
        request.Rol,
        request.Estado);

    public static ActualizarUsuarioCommand ToCommand(this ActualizarUsuarioRequest request, int id) => new(
        id,
        request.Nombre,
        request.Apellido,
        request.Usuario,
        request.Contrasena,
        request.Rol,
        request.Estado);

    public static IniciarSesionCommand ToCommand(this LoginRequest request) => new(
        request.Usuario,
        request.Contrasena);

    public static UsuarioResponse ToResponse(this UsuarioDetalle detalle) => new(
        detalle.Id,
        detalle.Nombre,
        detalle.Apellido,
        detalle.Usuario,
        detalle.Contrasena,
        detalle.Rol,
        detalle.Estado);
}