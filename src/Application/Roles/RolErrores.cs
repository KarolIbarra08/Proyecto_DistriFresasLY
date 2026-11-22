using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Roles;

public static class RolErrores
{
    public static Error NoEncontrado(int id) => Error.NotFound(
        "Rol.NotFound",
        $"No se encontró un rol con el Id: {id}");

    public static Error Duplicado(string nombre) => Error.Conflict(
        "Rol.Duplicado",
        $"El rol '{nombre}' ya se encuentra registrado.");

    public static Error TieneUsuariosAsociados(int id) => Error.Conflict(
        "Rol.TieneUsuarios",
        $"No se puede eliminar el rol con ID {id} porque tiene usuarios asignados.");

    public static Error AccesoDenegado(string nombreRol, string permiso) => Error.Validation(
        "Rol.AccesoDenegado",
        $"El rol '{nombreRol}' no cuenta con el permiso '{permiso}'.");
}