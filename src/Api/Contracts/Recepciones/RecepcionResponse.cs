namespace DistriFresasLY.Api.Contracts.Recepciones;

public record RecepcionResponse(
    int Id,
    DateTime FechaRecepcion,
    int Cantidad,
    double ValorPago
);