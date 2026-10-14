namespace DistriFresasLY.Api.Contracts.DetalleRecepciones;

public record DetalleRecepcionResponse(
    int Id,
    int IdRecepcion,
    int IdProducto,
    double Cantidad,
    double PrecioUnitario,
    double Subtotal,
    DateTime FechaIngreso
);