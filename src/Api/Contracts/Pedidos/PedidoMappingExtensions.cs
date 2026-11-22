using DistriFresasLY.Application.Pedidos;

namespace DistriFresasLY.Api.Contracts.Pedidos;

public static class PedidoMappingExtensions
{
    public static RegistrarPedidoCommand ToCommand(this CrearPedidoRequest request) => new(
        request.ProveedorId,
        request.FechaPedido,
        request.Estado,
        request.Detalles?.Select(d => new CrearDetallePedidoCommand(
            d.Descripcion,
            d.Cantidad,
            d.ValorUnitario)).ToList());

    public static ActualizarPedidoCommand ToCommand(this ActualizarPedidoRequest request, int id) => new(
        id,
        request.ProveedorId,
        request.Estado,
        request.Detalles?.Select(d => new CrearDetallePedidoCommand(
            d.Descripcion,
            d.Cantidad,
            d.ValorUnitario)).ToList());

    public static PedidoResponse ToResponse(this PedidoDetalleDto dto) => new(
        dto.Id,
        dto.ProveedorId,
        dto.FechaPedido,
        dto.Estado,
        dto.Total,
        dto.Detalles.Select(d => new DetallePedidoResponse(
            d.Id,
            d.PedidoId,
            d.Descripcion,
            d.Cantidad,
            d.ValorUnitario,
            d.Subtotal)).ToList());
}