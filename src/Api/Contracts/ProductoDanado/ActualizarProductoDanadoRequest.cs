namespace DistriFresasLY.Api.Contracts.ProductosDanados;

public record ActualizarProductoDanadoRequest(
    int Cantidad,
    string Identificacion,
    decimal Valor,
    DateTime FechaProductoDanado,
    string Motivo
);