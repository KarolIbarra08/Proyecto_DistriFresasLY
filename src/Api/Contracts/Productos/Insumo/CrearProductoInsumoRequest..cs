namespace DistriFresasLY.Api.Contracts.Productos;

public record CrearProductoInsumoRequest(
    string Nombre,
    string Tipo,
    string UnidadMedida,
    string Descripcion,
    int CantidadInicial 
);