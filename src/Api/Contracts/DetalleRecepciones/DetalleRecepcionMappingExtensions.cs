using DistriFresasLY.Application.DetalleRecepciones;
using DistriFresasLY.Application.Recepciones;

namespace DistriFresasLY.Api.Contracts.DetalleRecepciones;

public static class DetalleRecepcionMappingExtensions
{
    public static RegistrarDetalleRecepcionCommand ToCommand(this RegistrarDetalleRecepcionRequest request) => new(
        request.IdRecepcion,
        request.IdProducto,
        request.Cantidad,
        request.PrecioUnitario,
        request.FechaIngreso);

    public static ActualizarDetalleRecepcionCommand ToCommand(this ActualizarDetalleRecepcionRequest request, int id) => new(
        id,
        request.Cantidad,
        request.PrecioUnitario);

    public static DetalleRecepcionResponse ToResponse(this DetalleRecepcionItemDto d) => new(
        d.Id,
        d.IdRecepcion,
        d.IdProducto,
        d.Cantidad,
        d.PrecioUnitario,
        d.Subtotal,
        d.FechaIngreso);
}