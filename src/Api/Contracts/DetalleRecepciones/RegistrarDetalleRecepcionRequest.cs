namespace DistriFresasLY.Api.Contracts.DetalleRecepciones;

public record RegistrarDetalleRecepcionRequest(
    int IdRecepcion,
    int IdProducto,
    int Cantidad,
    double PrecioUnitario,
    DateTime? FechaIngreso = null
);