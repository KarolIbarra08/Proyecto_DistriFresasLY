using DistriFresasLY.Application.Ventas;

namespace DistriFresasLY.Api.Contracts.Ventas;

public static class VentaMappingExtensions
{
    public static RegistrarVentaCommand ToCommand(this CrearVentaRequest request, bool esAdministrador) => new(
        request.ClienteId,
        request.Detalles?.Select(d => new CrearDetalleVentaCommand(
            d.ProductoId,
            d.Cantidad,
            d.PrecioUnitario)).ToList(),
        esAdministrador);

    public static ActualizarVentaCommand ToCommand(this ActualizarVentaRequest request, int id) => new(
        id,
        request.ClienteId,
        request.Estado,
        request.Detalles?.Select(d => new CrearDetalleVentaCommand(
            d.ProductoId,
            d.Cantidad,
            d.PrecioUnitario)).ToList());

    public static VentaResponse ToResponse(this VentaDetalleDto detalle) => new(
        detalle.Id,
        detalle.FechaVenta,
        detalle.Total,
        detalle.Estado,
        detalle.ClienteId);

    public static DetalleVentaResponse ToResponse(this DetalleVentaDto detalle) => new(
        detalle.ProductoId,
        detalle.Cantidad,
        detalle.PrecioUnitario,
        detalle.Subtotal,
        detalle.TotalVenta);
}
