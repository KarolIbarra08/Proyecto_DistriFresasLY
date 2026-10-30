namespace DistriFresasLY.Api.Contracts.Productos;

public record CrearProductoInsumoRequest(
    string Nombre,
    string TipoInsumo,
    string UnidadMedida,
    string Descripcion
);