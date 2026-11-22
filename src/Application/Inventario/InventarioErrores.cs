using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Inventarios;

public static class InventarioErrores
{
    public static Error NoEncontradoPorProducto(int productoId) => Error.NotFound(
        "Inventario.NotFound",
        $"No se encontró registro de inventario para el producto ID: {productoId}");

    public static Error ProductoNoEncontrado(int productoId) => Error.NotFound(
        "Producto.NotFound",
        $"No existe el producto con ID: {productoId}");

    public static readonly Error ErrorCreacion = Error.Failure(
        "Inventario.ErrorCreacion",
        "No se pudo recuperar el registro de inventario creado.");
}