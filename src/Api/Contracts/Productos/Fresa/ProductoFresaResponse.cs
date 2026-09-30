namespace DistriFresasLY.Api.Contracts.Productos;

public record ProductoFresaResponse(
    int Id,
    string TipoProducto,
    int Calibre,
    string Calidad,
    double Peso,
    decimal PrecioCompra,
    DateTime FechaIngreso,
    string Descripcion,
    int CantidadInicial
);