namespace DistriFresasLY.Api.Contracts.ProductosDanados;

public record RegistrarProductoDanadoRequest(
    int ProductoId,
    string TipoProducto, 
    int Cantidad,
    string Identificacion,
    decimal Valor,
    DateTime? FechaProductoDanado,
    string Motivo
);