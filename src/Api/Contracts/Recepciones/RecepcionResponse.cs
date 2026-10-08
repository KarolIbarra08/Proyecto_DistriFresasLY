namespace DistriFresasLY.Api.Contracts.Recepciones;

public record RecepcionResponse(
    int Id,
    DateTime FechaRecepcion,
    double ValorPago
);