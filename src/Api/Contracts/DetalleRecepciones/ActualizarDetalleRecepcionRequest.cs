namespace DistriFresasLY.Api.Contracts.DetalleRecepciones;

public record ActualizarDetalleRecepcionRequest(
    int Cantidad,
    double PrecioUnitario
);