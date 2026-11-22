using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.DetalleRecepciones;

public static class DetalleRecepcionErrores
{
    public static Error NoEncontrado(int id) => Error.NotFound(
        "DetalleRecepcion.NotFound",
        $"No se encontró el detalle con ID: {id}");

    public static readonly Error IdsInvalidos = Error.Validation(
        "DetalleRecepcion.IdsInvalidos",
        "El IdRecepcion y el IdProducto deben ser mayores a 0.");

    public static readonly Error CantidadInvalida = Error.Validation(
        "DetalleRecepcion.CantidadInvalida",
        "La cantidad debe ser mayor a 0.");

    public static readonly Error PrecioInvalido = Error.Validation(
        "DetalleRecepcion.PrecioInvalido",
        "El precio unitario debe ser mayor a 0.");

    public static readonly Error RecepcionProcesada = Error.Validation(
        "DetalleRecepcion.RecepcionProcesada",
        "No se pueden modificar detalles de una recepción que ya fue procesada en inventario.");

    public static Error RecepcionNoEncontrada(int idRecepcion) => Error.NotFound(
        "Recepcion.NotFound",
        $"No se encontró la recepción con ID: {idRecepcion}.");

    public static Error ProductoNoEncontrado(int idProducto) => Error.NotFound(
        "Producto.NotFound",
        $"No se encontró el producto con ID: {idProducto}.");

    public static readonly Error ErrorCreacion = Error.Failure(
        "DetalleRecepcion.ErrorCreacion",
        "No se pudo recuperar el detalle creado.");

    public static readonly Error ErrorActualizacion = Error.Failure(
        "DetalleRecepcion.ErrorActualizacion",
        "No se pudo recuperar el detalle actualizado.");
}