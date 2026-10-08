namespace DistriFresasLY.Api.Contracts.DetalleRecepciones;

public record DetalleRecepcionResponse(
    int Id,
    int IdRecepcion,
    int IdProducto,
    double Cantidad,
    double UnidadMedida,
    string TipoMedida,
    double PrecioUnitario,
    double Subtotal
);