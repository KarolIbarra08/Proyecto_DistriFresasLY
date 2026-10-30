namespace DistriFresasLY.Api.Contracts.Productos;

public record ProductoInsumoResponse(
    int Id,
    string TipoProducto,
    string Nombre,
    string TipoInsumo,
    string UnidadMedida,
    string Descripcion
);