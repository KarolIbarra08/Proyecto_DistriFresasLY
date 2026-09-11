namespace DistriFresasLY.Api.Contracts.Recibos;

public record GenerarReciboRequest(
    int VentaId
);

public record ReciboResponse(
    int Id,
    DateTime FechaRecibo,
    int NumeroRecibo,
    decimal Total,
    int VentaId
);