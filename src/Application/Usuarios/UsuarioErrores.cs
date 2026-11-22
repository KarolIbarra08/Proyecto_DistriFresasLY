using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Usuarios;

public static class UsuarioErrores
{
    public static Error NoEncontrado(int id) => Error.NotFound(
        "Usuario.NotFound",
        $"No se encontró un usuario con el Id: {id}");

    public static Error Duplicado(string nombreUsuario) => Error.Conflict(
        "Usuario.Duplicado",
        $"El nombre de usuario '{nombreUsuario}' ya está en uso.");

    public static Error RolNoEncontrado(string rol) => Error.NotFound(
        "Usuario.RolNoEncontrado",
        $"El rol '{rol}' especificado no existe en el sistema.");

    public static readonly Error CredencialesObligatorias = Error.Validation(
        "Usuario.Validacion",
        "El usuario y la contraseña son obligatorios.");

    public static readonly Error CredencialesInvalidas = Error.NotFound(
        "Usuario.CredencialesInvalidas",
        "Usuario o contraseña incorrectos.");

    public static readonly Error Inactivo = Error.Validation(
        "Usuario.Inactivo",
        "El usuario se encuentra inactivo.");
}