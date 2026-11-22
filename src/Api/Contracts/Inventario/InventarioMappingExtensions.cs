using DistriFresasLY.Application.Inventarios;

namespace DistriFresasLY.Api.Contracts.Inventario;

public static class InventarioMappingExtensions
{
    public static ActualizarStockCommand ToCommand(this ActualizarStockRequest request) => new(
        request.ProductoId,
        request.Cantidad);

    public static InventarioResponse ToResponse(this InventarioDetalleDto d) => new(
        d.Id,
        d.ProductoId,
        d.CantidadDisponible,
        d.FechaActualizacion);
}