using DistriFresasLY.Application.Recibos;

namespace DistriFresasLY.Api.Contracts.Recibos;

public static class ReciboMappingExtensions
{
    public static GenerarReciboCommand ToCommand(this GenerarReciboRequest request) => new(
        request.VentaId);

    public static ReciboResponse ToResponse(this ReciboDetalleDto detalle) => new(
        detalle.Id,
        detalle.FechaRecibo,
        detalle.NumeroRecibo,
        detalle.Total,
        detalle.VentaId);
}
