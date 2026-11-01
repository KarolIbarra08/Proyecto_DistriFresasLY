namespace DistriFresasLY.Api.Contracts.Recepciones;

public record RegistrarRecepcionRequest(
    int PedidoId ,
    DateTime? FechaRecepcion = null
);
