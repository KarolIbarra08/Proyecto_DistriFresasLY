using DistriFresasLY.Domain.Entities.Roles;

namespace DistriFresasLY.Api.Endpoints.Roles;

public static class RolDataStore
{
    public static readonly List<Rol> RolesDb =
    [
        Rol.Create(
            nombre: "Administrador",
            descripcion: "Acceso total a las funciones del sistema",
            permisos: ["ventas.crear", "ventas.eliminar", "usuarios.gestionar", "clientes.gestionar"],
            id: 1).Value,
        Rol.Create(
            nombre: "Vendedor",
            descripcion: "Acceso para registro de ventas y consulta de clientes",
            permisos: ["ventas.crear", "clientes.gestionar"],
            id: 2).Value
    ];
}