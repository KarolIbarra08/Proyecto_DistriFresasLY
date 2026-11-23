using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Recibos;

public static class ReciboErrores
{
    public static Error NoEncontrado(int id) => Error.NotFound(
        "Recibo.NotFound",
        $"No se encontro el recibo con Id: {id}");

    public static Error VentaNoEncontrada(int ventaId) => Error.NotFound(
        "Recibo.VentaNotFound",
        $"No se encontró la venta #{ventaId} para generar el recibo.");

    public static Error YaExiste(int ventaId, int numeroRecibo) => Error.Conflict(
        "Recibo.AlreadyExists",
        $"Ya existe un recibo (No. #{numeroRecibo}) generado para la venta #{ventaId}.");

    public static readonly Error ErrorCreacion = Error.Failure(
        "Recibo.ErrorCreacion",
        "No se pudo recuperar el recibo creado.");
}
