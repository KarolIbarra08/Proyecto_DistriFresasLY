namespace DistriFresasLY.Api.Contracts.Productos;

public record ProductoFresaResponse(
    int Id,
    string TipoProducto,
    int Calibre,
    string Calidad,
    double Peso,
    string Descripcion
);