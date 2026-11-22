using DistriFresasLY.Application.Clientes;

namespace DistriFresasLY.Api.Contracts.Clientes;

public static class ClienteMappingExtensions
{
    public static CrearClienteCommand ToCommand(this CrearClienteRequest request) => new(
        request.Nombre,
        request.CedulaNit,
        request.Telefono,
        request.Direccion,
        request.TipoNegocio);

    public static ActualizarClienteCommand ToCommand(this ActualizarClienteRequest request, int id) => new(
        id,
        request.Nombre,
        request.CedulaNit,
        request.Telefono,
        request.Direccion,
        request.TipoNegocio);

    public static ClienteResponse ToResponse(this ClienteDetalle detalle) => new(
        Id: detalle.Id,
        Nombre: detalle.Nombre,
        CedulaNit: detalle.CedulaNit,
        Telefono: detalle.Telefono,
        Direccion: detalle.Direccion,
        TipoNegocio: detalle.TipoNegocio);
}