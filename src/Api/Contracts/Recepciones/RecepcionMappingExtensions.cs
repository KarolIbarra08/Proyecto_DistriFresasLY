using DistriFresasLY.Application.Recepciones;

namespace DistriFresasLY.Api.Contracts.Recepciones;

public static class RecepcionMappingExtensions
{
    public static RegistrarRecepcionCommand ToCommand(this RegistrarRecepcionRequest request) => new(
        request.PedidoId,
        request.FechaRecepcion);

    public static ActualizarRecepcionCommand ToCommand(this ActualizarRecepcionRequest request, int id) => new(
        id,
        request.PedidoId);

    public static RecepcionResponse ToResponse(this RecepcionDetalleDto d) => new(
        d.Id,
        d.PedidoId,
        d.FechaRecepcion,
        d.ValorPago,
        d.ProcesadaEnInventario,
        d.TotalCalculado);
}