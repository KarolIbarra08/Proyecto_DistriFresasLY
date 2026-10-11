namespace DistriFresasLY.Api.Contracts.Recepciones;

public record ActualizarRecepcionRequest(
    decimal ValorPago,
    DateTime? FechaRecepcion = null
);