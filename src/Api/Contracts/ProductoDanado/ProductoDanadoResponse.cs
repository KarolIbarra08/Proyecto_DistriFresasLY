using DistriFresasLY.Api.Contracts.Descuentos;

namespace DistriFresasLY.Api.Contracts.ProductosDanados;

public record ProductoDanadoResponse(
    int Id,
    int ProductoId,
    string TipoProducto, 
    int Cantidad,
    string Identificacion,
    double Valor,
    DateTime FechaProductoDanado,
    string Motivo,
    DescuentoResponse? Descuento
);