namespace DistriFresasLY.Api.Contracts.Descuentos;

public record DescuentoResponse(
    decimal Valor,
    DateTime Fecha,
    string Motivo
);