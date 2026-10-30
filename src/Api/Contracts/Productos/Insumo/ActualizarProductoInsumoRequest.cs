namespace DistriFresasLY.Api.Contracts.Productos;

public record ActualizarProductoInsumoRequest(
    string Nombre,
    string TipoInsumo,
    string UnidadMedida,
    string Descripcion
);