namespace DistriFresasLY.Api.Contracts.Descuentos;

public record DescuentoResponse(
    double Valor,
    DateTime Fecha,
    string Motivo
);