using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Clientes;

public static class ClienteErrores
{
    public static Error NoEncontrado(int id) => Error.NotFound(
        "Cliente.NotFound",
        $"No se encontró un cliente con el Id: {id}");

    public static readonly Error CedulaNitDuplicada = Error.Conflict(
        "Cliente.CedulaNitDuplicada",
        "La Cédula/NIT ya está registrada por otro cliente.");

    public static Error TienePedidosAsociados(int id) => Error.Conflict(
        "Cliente.TienePedidos",
        $"No se puede eliminar el cliente con ID {id} porque tiene pedidos asociados.");
}