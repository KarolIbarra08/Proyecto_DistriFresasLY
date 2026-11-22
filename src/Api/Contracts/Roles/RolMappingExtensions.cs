using DistriFresasLY.Application.Roles;

namespace DistriFresasLY.Api.Contracts.Roles;

public static class RolMappingExtensions
{
    public static CrearRolCommand ToCommand(this CrearRolRequest request) => new(
        request.Nombre,
        request.Descripcion);

    public static ActualizarRolCommand ToCommand(this ActualizarRolRequest request, int id) => new(
        id,
        request.Nombre,
        request.Descripcion);

    public static AsignarPermisoCommand ToCommand(this AsignarPermisoRequest request, int idRol) => new(
        idRol,
        request.Permiso);

    public static VerificarAccesoQuery ToQuery(this VerificarAccesoRequest request, int idRol) => new(
        idRol,
        request.Permiso);

    public static RolResponse ToResponse(this RolDetalle detalle) => new(
        detalle.Id,
        detalle.Nombre,
        detalle.Descripcion,
        detalle.Permisos.ToList());
}