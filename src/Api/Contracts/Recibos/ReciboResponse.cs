namespace DistriFresasLY.Api.Contracts.Recibos;
public record ReciboResponse(
    int Id,
    DateTime FechaRecibo,
    int NumeroRecibo,
    decimal Total,
    int VentaId
);