using DistriFresasLY.Application.Pagos;

namespace DistriFresasLY.Api.Contracts.Pagos.Clientes;

public static class PagoClienteMappingExtensions
{
    public static RegistrarPagoClienteCommand ToCommand(this CrearPagoClienteRequest request) => new(
        request.ClienteId,
        request.VentaId,
        request.Valor,
        request.MedioPago,
        request.FechaPago);

    public static ActualizarPagoClienteCommand ToCommand(this ActualizarPagoClienteRequest request, int id) => new(
        id,
        request.ClienteId,
        request.VentaId,
        request.Valor,
        request.MedioPago,
        request.FechaPago,
        request.Estado);

    public static PagoClienteResponse ToResponse(this PagoClienteDetalleDto detalle) => new(
        detalle.Id,
        detalle.ClienteId,
        detalle.VentaId,
        detalle.Valor,
        detalle.MedioPago,
        detalle.FechaPago,
        detalle.Estado);
}
