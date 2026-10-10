namespace DistriFresasLY.Api.Contracts.ProductosDanados;

public record RegistrarProductoDanadoRequest(
    int ProductoId,
    string TipoProducto, 
    int Cantidad,
    string Identificacion,
    double Valor,
    DateTime? FechaProductoDanado,
    string Motivo
);