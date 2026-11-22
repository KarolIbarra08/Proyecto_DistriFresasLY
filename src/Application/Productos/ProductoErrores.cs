using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Productos;

public static class ProductoErrores
{
    public static Error FresaNoEncontrada(int id) => Error.NotFound(
        "ProductoFresa.NotFound",
        $"No se encontró el producto fresa con ID: {id}");

    public static Error InsumoNoEncontrado(int id) => Error.NotFound(
        "ProductoInsumo.NotFound",
        $"No se encontró el producto insumo con ID: {id}");
}