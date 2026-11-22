namespace DistriFresasLY.Application.ProductosDanados;

public sealed record ProductoDanadoDetalle(
    int Id,
    int ProductoId,
    string TipoProducto,
    int Cantidad,
    string Identificacion,
    double Valor,
    DateTime FechaProductoDanado,
    string Motivo);

public sealed record DescuentoCalculadoDetalle(
    decimal Valor,
    DateTime Fecha,
    string Motivo);

public sealed record CalculoDescuentoDetalle(
    int Id,
    int ProductoId,
    int RecepcionIdAsociada,
    string TipoProducto,
    int Cantidad,
    double PrecioUnitario,
    DateTime FechaProductoDanado,
    string Motivo,
    DescuentoCalculadoDetalle? Descuento);