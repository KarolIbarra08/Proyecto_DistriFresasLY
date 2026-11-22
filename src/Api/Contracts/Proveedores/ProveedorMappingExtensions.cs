using DistriFresasLY.Application.Proveedores;

namespace DistriFresasLY.Api.Contracts.Proveedores;

public static class ProveedorMappingExtensions
{
    public static CrearProveedorCommand ToCommand(this CrearProveedorRequest request) => new(
        request.Nombre,
        request.Apellido,
        request.CedulaNit,
        request.Telefono,
        request.Direccion,
        request.NombreEmpresa);

    public static ActualizarProveedorCommand ToCommand(this ActualizarProveedorRequest request, int id) => new(
        id,
        request.Nombre,
        request.Apellido,
        request.CedulaNit,
        request.Telefono,
        request.Direccion,
        request.NombreEmpresa);

    public static ProveedorResponse ToResponse(this ProveedorDetalle detalle) => new(
        Id: detalle.Id,
        Nombre: detalle.Nombre,
        Apellido: detalle.Apellido,
        CedulaNit: detalle.CedulaNit,
        Telefono: detalle.Telefono,
        Direccion: detalle.Direccion,
        NombreEmpresa: detalle.NombreEmpresa);
}