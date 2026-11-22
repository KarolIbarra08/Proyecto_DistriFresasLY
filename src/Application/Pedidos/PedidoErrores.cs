using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pedidos;

public static class PedidoErrores
{
    public static Error NoEncontrado(int id) => Error.NotFound(
        "Pedido.NotFound",
        $"No se encontró un pedido registrado con el ID '{id}'.");

    public static readonly Error ProveedorInvalido = Error.Validation(
        "Pedido.ProveedorInvalido",
        "El ID del proveedor es obligatorio.");

    public static Error ProveedorNoEncontrado(int proveedorId) => Error.NotFound(
        "Pedido.ProveedorNoEncontrado",
        $"No se encontró un proveedor con ID {proveedorId}.");

    public static readonly Error SinDetalles = Error.Validation(
        "Pedido.SinDetalles",
        "El pedido debe incluir al menos un producto en el detalle.");

    public static readonly Error ConPagosAsociadosEliminar = Error.Conflict(
        "Pedido.PagosAsociados",
        "No se puede eliminar un pedido con pagos asociados.");

    public static readonly Error ConPagosAsociadosCambiarProveedor = Error.Conflict(
        "Pedido.PagosAsociados",
        "No se puede cambiar el proveedor de un pedido con pagos asociados.");

    public static readonly Error ErrorCreacion = Error.Failure(
        "Pedido.ErrorCreacion",
        "No se pudo recuperar el pedido creado.");
}