namespace DistriFresasLY.Api.Contracts.Recepciones;

public record RegistrarRecepcionRequest(
    decimal ValorPago,
    DateTime? FechaRecepcion = null
);