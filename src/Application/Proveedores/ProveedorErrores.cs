using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Proveedores;

public static class ProveedorErrores
{
    public static Error NoEncontrado(int id) => Error.NotFound(
        "Proveedor.NotFound",
        $"No se encontró un proveedor con el Id: {id}");

    public static Error CedulaDuplicada(string cedulaNit) => Error.Conflict(
        "Proveedor.CedulaDuplicada",
        $"Ya existe un proveedor registrado con la Cédula/NIT '{cedulaNit}'.");

    public static readonly Error DatosObligatorios = Error.Validation(
        "Proveedor.Validacion",
        "El nombre y la Cédula/NIT son campos obligatorios.");

    public static Error TienePedidosAsociados(int id) => Error.Conflict(
        "Proveedor.TienePedidos",
        $"No se puede eliminar el proveedor con ID {id} porque tiene pedidos o pagos asociados.");
}