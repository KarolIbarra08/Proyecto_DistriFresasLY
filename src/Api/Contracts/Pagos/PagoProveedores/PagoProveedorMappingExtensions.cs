using DistriFresasLY.Application.Pagos;

namespace DistriFresasLY.Api.Contracts.Pagos.Proveedores;

public static class PagoProveedorMappingExtensions
{
    public static RegistrarPagoProveedorCommand ToCommand(this CrearPagoProveedorRequest request) => new(
        request.ProveedorId,
        request.PedidoId,
        request.Valor,
        request.MedioPago,
        request.FechaPago);

    public static ActualizarPagoProveedorCommand ToCommand(this ActualizarPagoProveedorRequest request, int id) => new(
        id,
        request.ProveedorId,
        request.PedidoId,
        request.Valor,
        request.MedioPago,
        request.FechaPago,
        request.Estado);

    public static PagoProveedorResponse ToResponse(this PagoProveedorDetalleDto detalle) => new(
        detalle.Id,
        detalle.ProveedorId,
        detalle.PedidoId,
        detalle.Valor,
        detalle.MedioPago,
        detalle.FechaPago,
        detalle.Estado);
}
