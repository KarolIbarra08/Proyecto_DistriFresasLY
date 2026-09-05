namespace DistriFresasLY.Api.Endpoints.Roles;

public record RolModel(
    int Id,
    string Nombre,
    string Descripcion,
    List<string> Permisos
);

public static class RolDataStore
{
    public static readonly List<RolModel> RolesDb =
    [
        new(
            Id: 1,
            Nombre: "Administrador",
            Descripcion: "Acceso total a las funciones del sistema",
            Permisos: ["ventas.crear", "ventas.eliminar", "usuarios.gestionar", "clientes.gestionar"]
        ),
        new(
            Id: 2,
            Nombre: "Vendedor",
            Descripcion: "Acceso para registro de ventas y consulta de clientes",
            Permisos: ["ventas.crear", "clientes.gestionar"]
        )
    ];
}