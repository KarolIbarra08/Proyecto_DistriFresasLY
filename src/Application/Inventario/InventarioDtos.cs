namespace DistriFresasLY.Application.Inventarios;

public sealed record InventarioDetalleDto(
    int Id,
    int ProductoId,
    int CantidadDisponible,
    DateTime FechaActualizacion);