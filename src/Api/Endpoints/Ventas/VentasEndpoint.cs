namespace DistriFresasLY.Api.Endpoints.Ventas;

public record DetalleVentaModel(
    int ProductoId,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal
);

public record VentaModel(
    int Id,
    DateTime FechaVenta,
    decimal Total,
    string Estado,
    int? ClienteId,
    List<DetalleVentaModel>? Detalles = null
);

public static class VentaDataStore
{
    public static readonly List<VentaModel> VentasDb =
    [
        new(
            Id: 1,
            FechaVenta: DateTime.Now.AddDays(-1),
            Total: 150000m,
            Estado: "Completada",
            ClienteId: 1,
            Detalles:
            [
                new(
                    ProductoId: 101, 
                    Cantidad: 10, 
                    PrecioUnitario: 15000m, 
                    Subtotal: 150000m
                )
            ]
        )
    ];
}