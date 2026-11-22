using DistriFresasLY.Application.Productos;

namespace DistriFresasLY.Api.Contracts.Productos;

public static class ProductoMappingExtensions
{
    public static RegistrarProductoFresaCommand ToCommand(this CrearProductoFresaRequest request) => new(
        request.Calibre,
        request.Calidad,
        request.Peso,
        request.Descripcion);

    public static ActualizarProductoFresaCommand ToCommand(this ActualizarProductoFresaRequest request, int id) => new(
        id,
        request.Calibre,
        request.Calidad,
        request.Peso,
        request.Descripcion);

    public static ProductoFresaResponse ToResponse(this ProductoFresaDetalle d) => new(
        d.Id,
        d.TipoProducto,
        d.Calibre,
        d.Calidad,
        d.Peso,
        d.Descripcion);

    public static RegistrarProductoInsumoCommand ToCommand(this CrearProductoInsumoRequest request) => new(
        request.Nombre,
        request.TipoInsumo,
        request.UnidadMedida,
        request.Descripcion);

    public static ActualizarProductoInsumoCommand ToCommand(this ActualizarProductoInsumoRequest request, int id) => new(
        id,
        request.Nombre,
        request.TipoInsumo,
        request.UnidadMedida,
        request.Descripcion);

    public static ProductoInsumoResponse ToResponse(this ProductoInsumoDetalle d) => new(
        d.Id,
        d.TipoProducto,
        d.Nombre,
        d.TipoInsumo,
        d.UnidadMedida,
        d.Descripcion);
}