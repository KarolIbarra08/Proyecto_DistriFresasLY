namespace DistriFresasLY.Api.Contracts.Recepciones;

public record RegistrarRecepcionRequest(
    double ValorPago,
    DateTime? FechaRecepcion = null
);