using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pagos;

public static class PagoProveedorErrores
{
    public static Error NoEncontrado(int id) => Error.NotFound(
        "PagoProveedor.NotFound",
        $"No se encontro un pago de proveedor registrado con el ID '{id}'.");

    public static readonly Error ValorInvalido = Error.Validation(
        "PagoProveedor.ValorInvalido",
        "El valor del pago debe ser mayor a 0.");

    public static Error PedidoNoEncontrado(int pedidoId) => Error.NotFound(
        "Pedido.NotFound",
        $"No existe el pedido con ID {pedidoId}.");

    public static readonly Error ProveedorNoCoincide = Error.Validation(
        "PagoProveedor.ProveedorNoCoincide",
        "El proveedor del pago no coincide con el proveedor del pedido.");

    public static Error PedidoCerrado(int pedidoId, string estado) => Error.Validation(
        "PagoProveedor.PedidoCerrado",
        $"No se pueden registrar pagos porque el pedido #{pedidoId} se encuentra en estado '{estado}'.");

    public static readonly Error PedidoSaldado = Error.Validation(
        "PagoProveedor.PedidoSaldado",
        "Este pedido ya se encuentra totalmente pagado.");

    public static Error MontoExcedido(double valor, double saldo) => Error.Validation(
        "PagoProveedor.MontoExcedido",
        $"El pago (${valor}) supera el saldo pendiente del pedido (${saldo}).");

    public static readonly Error ErrorCreacion = Error.Failure(
        "PagoProveedor.ErrorCreacion",
        "No se pudo recuperar el pago creado.");
}
