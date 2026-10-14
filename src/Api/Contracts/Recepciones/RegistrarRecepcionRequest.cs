namespace DistriFresasLY.Api.Contracts.Recepciones;

public record RegistrarRecepcionRequest(
    int IdProveedor,
    DateTime? FechaRecepcion = null
);