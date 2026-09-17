namespace DistriFresasLY.Api.Endpoints.Recibos;

public record ReciboModel(
    int Id,
    DateTime FechaRecibo,
    int NumeroRecibo,
    decimal Total,
    int VentaId
);

public static class ReciboDataStore
{
    public static readonly List<ReciboModel> RecibosDb = [];
}