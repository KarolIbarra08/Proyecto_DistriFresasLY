namespace DistriFresasLY.Api.Contracts.Productos;

public record ProductoInsumoResponse(
    int Id,
    string TipoProducto,
    string Nombre,
    string Tipo,
    string UnidadMedida,
    string Descripcion,
    DateTime FechaIngreso
);