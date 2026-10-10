namespace DistriFresasLY.Api.Contracts.DetalleRecepciones;

public record ActualizarDetalleRecepcionRequest(
    int Cantidad,
    double UnidadMedida,
    string TipoMedida,
    double PrecioUnitario
);