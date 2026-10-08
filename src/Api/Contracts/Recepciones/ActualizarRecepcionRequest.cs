namespace DistriFresasLY.Api.Contracts.Recepciones;

public record ActualizarRecepcionRequest(
    int Cantidad,
    double ValorPago,
    DateTime? FechaRecepcion = null
);