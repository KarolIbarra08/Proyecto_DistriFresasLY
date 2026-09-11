namespace DistriFresasLY.Api.Endpoints.Inventario;

public record InventarioModel(
    int Id,
    int ProductoId,
    int CantidadDisponible,
    DateTime FechaActualizacion
);

public static class InventarioDataStore
{
    public static readonly List<InventarioModel> InventarioDb =
    [
        new(
            Id: 1,
            ProductoId: 101,
            CantidadDisponible: 50,
            FechaActualizacion: DateTime.Now
        )
    ];
}