using DistriFresasLY.Domain.Entities;

namespace DistriFresasLY.Api.Endpoints.Inventario;

public static class InventarioDataStore
{
    public static readonly List<Domain.Entities.Inventario> InventarioDb =
    [
        new Domain.Entities.Inventario(1, 101, 50)
    ];

    public static Domain.Entities.Inventario? ObtenerPorProductoId(int productoId)
    {
        return InventarioDb.FirstOrDefault(i => i.ProductoId == productoId);
    }

    public static Domain.Entities.Inventario? AjustarStock(int productoId, int cantidad)
    {
        var item = ObtenerPorProductoId(productoId);
        item?.Ajustar(cantidad);
        return item;
    }

    public static Domain.Entities.Inventario? AumentarStock(int productoId, int cantidad)
    {
        var item = ObtenerPorProductoId(productoId);
        item?.Aumentar(cantidad);
        return item;
    }

    public static (bool Exito, Domain.Entities.Inventario? Item, string? Error) DisminuirStock(int productoId, int cantidad)
    {
        var item = ObtenerPorProductoId(productoId);
        if (item is null)
        {
            return (false, null, $"No existe registro de inventario para el producto: {productoId}");
        }

        if (!item.PuedeDisminuir(cantidad))
        {
            return (false, item, $"El stock actual ({item.CantidadDisponible}) es insuficiente para descontar {cantidad} unidades.");
        }

        item.Disminuir(cantidad);
        return (true, item, null);
    }
}