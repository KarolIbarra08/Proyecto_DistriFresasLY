namespace DistriFresasLY.Api.Contracts.Productos;

public record CrearProductoFresaRequest(
    int Calibre,
    string Calidad,
    double Peso,
    string Descripcion
);