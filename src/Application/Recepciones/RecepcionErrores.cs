using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Recepciones;

public static class RecepcionErrores
{
    public static Error NoEncontrada(int id) => Error.NotFound(
        "Recepcion.NotFound",
        $"No se encontró una recepción con el Id: {id}");

    public static readonly Error YaProcesada = Error.Validation(
        "Recepcion.YaProcesada",
        "No se puede editar una recepción cuyo inventario ya ha sido procesado.");

    public static readonly Error PedidoIdInvalido = Error.Validation(
        "Recepcion.PedidoIdInvalido",
        "El PedidoId es obligatorio y debe ser mayor a 0.");

    public static Error PedidoNoEncontrado(int pedidoId) => Error.NotFound(
        "Pedido.NotFound",
        $"No existe el pedido con ID {pedidoId}.");

    public static readonly Error ErrorCreacion = Error.Failure(
        "Recepcion.ErrorCreacion",
        "No se pudo recuperar la recepción creada.");
}