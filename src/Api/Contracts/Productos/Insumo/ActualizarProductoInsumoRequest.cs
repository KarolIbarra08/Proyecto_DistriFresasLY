namespace DistriFresasLY.Api.Contracts.Productos;

public record ActualizarProductoInsumoRequest(
    string Nombre,
    string Tipo,
    string UnidadMedida,
    string Descripcion
);