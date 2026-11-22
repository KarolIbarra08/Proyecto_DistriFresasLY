using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.ProductosDanados;

public static class ProductoDanadoErrores
{
    public static Error NoEncontrado(int id) => Error.NotFound(
        "ProductoDanado.NotFound",
        $"No se encontró el producto dañado con ID: {id}");

    public static readonly Error DatosInvalidos = Error.Validation(
        "ProductoDanado.DatosInvalidos",
        "El ProductoId y la Cantidad deben ser mayores a 0.");

    public static Error ProductoNoEncontrado(int productoId) => Error.NotFound(
        "Producto.NotFound",
        $"No se encontró un producto con ID: {productoId}.");

    public static Error InventarioNoEncontrado(int productoId) => Error.NotFound(
        "Inventario.NotFound",
        $"No se encontró registro de inventario para el producto con ID: {productoId}.");

    public static Error RecepcionNoEncontradaEspecifica(int recepcionId, int productoId) => Error.NotFound(
        "Recepcion.NotFound",
        $"La recepción ID {recepcionId} no existe o no contiene el producto ID {productoId}.");

    public static Error RecepcionNoEncontradaProducto(int productoId) => Error.NotFound(
        "Recepcion.NotFound",
        $"No se encontró ninguna recepción asociada al producto ID {productoId}.");
}