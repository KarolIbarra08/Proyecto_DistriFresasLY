namespace DistriFresasLY.Api.Contracts.Recepciones;

public record RegistrarRecepcionRequest(
    int Cantidad,
    double ValorPago,
    DateTime? FechaRecepcion = null
);