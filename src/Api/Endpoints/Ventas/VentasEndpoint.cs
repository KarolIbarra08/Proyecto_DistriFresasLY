namespace DistriFresasLY.Api.Endpoints.Ventas;

public record VentaModel(
    int Id,
    DateTime FechaVenta,
    decimal Total,
    string Estado,
    int? ClienteId
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
            ClienteId: 1
        )
    ];
}