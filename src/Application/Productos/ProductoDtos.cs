namespace DistriFresasLY.Application.Productos;

public sealed record ProductoFresaDetalle(
    int Id,
    string TipoProducto,
    int Calibre,
    string Calidad,
    double Peso,
    string Descripcion);

public sealed record ProductoInsumoDetalle(
    int Id,
    string TipoProducto,
    string Nombre,
    string TipoInsumo,
    string UnidadMedida,
    string Descripcion);