namespace DistriFresasLY.Api.Contracts.DetalleRecepciones;

public record DetalleRecepcionResponse(
    int Id,
    int IdRecepcion,
    int IdProducto,
    int Cantidad,
    double UnidadMedida,
    string TipoMedida,
    decimal PrecioUnitario,
    decimal Subtotal
);