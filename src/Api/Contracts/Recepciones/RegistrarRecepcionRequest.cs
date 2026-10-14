namespace DistriFresasLY.Api.Contracts.Recepciones;

public record RegistrarRecepcionRequest(
    int? PedidoId = null,
    DateTime? FechaRecepcion = null
);