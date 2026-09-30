namespace DistriFresasLY.Api.Contracts.Productos;

public record ActualizarProductoFresaRequest(
    int Calibre,
    string Calidad,
    double Peso,
    decimal PrecioCompra,
    string Descripcion
);