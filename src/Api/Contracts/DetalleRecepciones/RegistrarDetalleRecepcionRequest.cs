namespace DistriFresasLY.Api.Contracts.DetalleRecepciones;

public record RegistrarDetalleRecepcionRequest(
    int IdRecepcion,
    int IdProducto,
    double Cantidad,
    double PrecioUnitario,
    DateTime? FechaIngreso = null
);