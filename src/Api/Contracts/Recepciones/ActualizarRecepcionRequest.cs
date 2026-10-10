namespace DistriFresasLY.Api.Contracts.Recepciones;

public record ActualizarRecepcionRequest(
    double ValorPago,
    DateTime? FechaRecepcion = null
);