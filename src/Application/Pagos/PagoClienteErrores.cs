using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Pagos;

public static class PagoClienteErrores
{
    public static Error NoEncontrado(int id) => Error.NotFound(
        "PagoCliente.NotFound",
        $"No se encontró el pago con ID {id}");

    public static readonly Error IdsInvalidos = Error.Validation(
        "PagoCliente.IdsInvalidos",
        "El ClienteId y VentaId deben ser obligatorios y mayores a 0.");

    public static readonly Error ValorInvalido = Error.Validation(
        "PagoCliente.ValorInvalido",
        "El valor del pago debe ser mayor a 0.");

    public static Error VentaNoEncontrada(int ventaId) => Error.NotFound(
        "Venta.NotFound",
        $"No existe la venta con ID {ventaId}.");

    public static readonly Error ClienteNoCoincide = Error.Validation(
        "PagoCliente.ClienteNoCoincide",
        "El ClienteId no coincide con el cliente de la venta.");

    public static Error VentaCerrada(int ventaId, string estado) => Error.Validation(
        "PagoCliente.VentaCerrada",
        $"No se pueden registrar pagos porque la venta #{ventaId} se encuentra en estado '{estado}'.");

    public static readonly Error VentaSaldada = Error.Validation(
        "PagoCliente.VentaSaldada",
        "Esta venta ya se encuentra totalmente pagada.");

    public static Error MontoExcedido(double valor, double saldo) => Error.Validation(
        "PagoCliente.MontoExcedido",
        $"El abono (${valor}) supera el saldo pendiente de la venta (${saldo}).");

    public static readonly Error ErrorCreacion = Error.Failure(
        "PagoCliente.ErrorCreacion",
        "No se pudo recuperar el pago creado.");
}
