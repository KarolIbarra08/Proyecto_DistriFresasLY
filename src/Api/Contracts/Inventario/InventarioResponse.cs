namespace DistriFresasLY.Api.Contracts.Inventario;

public record InventarioResponse(
    int Id,
    int ProductoId,
    int CantidadDisponible,
    DateTime FechaActualizacion
);