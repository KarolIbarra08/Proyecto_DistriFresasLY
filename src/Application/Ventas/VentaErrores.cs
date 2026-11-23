using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Ventas;

public static class VentaErrores
{
    public static Error NoEncontrada(int id) => Error.NotFound(
        "Venta.NotFound",
        $"No se encontró una venta con el ID: {id}");

    public static readonly Error ClienteRequerido = Error.Validation(
        "Venta.ClienteRequerido",
        "El cliente es obligatorio para registrar una venta.");

    public static Error ClienteNoEncontrado(int clienteId) => Error.NotFound(
        "Venta.ClienteNoEncontrado",
        $"No se encontró un cliente con ID {clienteId}.");

    public static readonly Error SinDetalles = Error.Validation(
        "Venta.SinDetalles",
        "La venta debe contener al menos un producto en el detalle.");

    public static Error InventarioNoEncontrado(int productoId) => Error.NotFound(
        "Inventario.NotFound",
        $"No existe registro de inventario para el producto ID: {productoId}");

    public static Error StockInsuficiente(int productoId, int disponible, int solicitado) => Error.Validation(
        "Venta.StockInsuficiente",
        $"No hay stock suficiente para el producto ID {productoId}. Disponible: {disponible}, solicitado: {solicitado}.");

    public static readonly Error ErrorCreacion = Error.Failure(
        "Venta.ErrorCreacion",
        "No se pudo recuperar la venta creada.");

    public static Error PagosAsociados(int id) => Error.Conflict(
        "Venta.PagosAsociados",
        $"No se puede eliminar la venta con ID {id} porque tiene pagos asociados.");

    public static Error ReciboAsociado(int id) => Error.Conflict(
        "Venta.ReciboAsociado",
        $"No se puede eliminar la venta con ID {id} porque tiene un recibo asociado.");
}
