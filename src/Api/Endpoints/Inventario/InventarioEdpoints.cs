using InventarioEntidad = DistriFresasLY.Domain.Entities.Inventario.Inventario;

namespace DistriFresasLY.Api.Endpoints.Inventario;

public static class InventarioDataStore
{
    public static readonly List<InventarioEntidad> InventarioDb = new();

    public static InventarioEntidad? ObtenerPorProductoId(int productoId)
    {
        return InventarioDb.FirstOrDefault(i => i.ProductoId == productoId);
    }

    public static void InicializarStock(int productoId, int cantidadInicial)
    {
        var item = ObtenerPorProductoId(productoId);

        if (item is null)
        {
            int nuevoId = InventarioDb.Count != 0 ? InventarioDb.Max(i => i.Id) + 1 : 1;
            var crearResult = InventarioEntidad.Create(productoId, cantidadInicial, nuevoId);
            if (crearResult.IsSuccess)
            {
                InventarioDb.Add(crearResult.Value);
            }
        }
        else
        {
            item.Ajustar(cantidadInicial);
        }
    }

    public static InventarioEntidad? AumentarStock(int productoId, int cantidad)
    {
        var item = ObtenerPorProductoId(productoId);

        if (item is null)
        {
            int nuevoId = InventarioDb.Count != 0 ? InventarioDb.Max(i => i.Id) + 1 : 1;
            var crearResult = InventarioEntidad.Create(productoId, cantidad, nuevoId);
            if (crearResult.IsFailure)
            {
                return null;
            }

            InventarioDb.Add(crearResult.Value);
            return crearResult.Value;
        }

        var aumentarResult = item.Aumentar(cantidad);
        return aumentarResult.IsSuccess ? item : null;
    }

    public static (bool exito, InventarioEntidad? inventario, string? mensajeError) DisminuirStock(int productoId, int cantidad)
    {
        var item = ObtenerPorProductoId(productoId);

        if (item is null)
        {
            return (false, null, $"No se encontró registro de inventario para el producto ID: {productoId}");
        }

        var disminuirResult = item.Disminuir(cantidad);
        if (disminuirResult.IsFailure)
        {
            return (false, item, disminuirResult.Error.Description);
        }

        return (true, item, null);
    }
}