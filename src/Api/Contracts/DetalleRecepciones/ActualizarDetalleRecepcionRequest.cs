namespace DistriFresasLY.Api.Contracts.DetalleRecepciones;

public record ActualizarDetalleRecepcionRequest(
    double Cantidad,
    double PrecioUnitario
);