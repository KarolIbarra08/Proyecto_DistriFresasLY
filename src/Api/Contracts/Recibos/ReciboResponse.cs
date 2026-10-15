namespace DistriFresasLY.Api.Contracts.Recibos;
public record ReciboResponse(
    int Id,
    DateTime FechaRecibo,
    int NumeroRecibo,
    double Total,
    int VentaId
);