namespace DistriFresasLY.Api.Contracts.ProductosDanados;

public record ActualizarProductoDanadoRequest(
    int Cantidad,
    string Identificacion,
    double Valor,
    DateTime FechaProductoDanado,
    string Motivo
);