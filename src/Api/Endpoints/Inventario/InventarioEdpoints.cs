using DistriFresasLY.Api.Contracts.Inventario;

namespace DistriFresasLY.Api.Endpoints.Inventario;

public static class InventarioDataStore
{
    public static readonly List<InventarioResponse> InventarioDb = new();

    public static InventarioResponse? ObtenerPorProductoId(int productoId)
    {
        return InventarioDb.FirstOrDefault(i => i.ProductoId == productoId);
    }

    public static void InicializarStock(int productoId, int cantidadInicial)
    {
        var item = ObtenerPorProductoId(productoId);

        if (item is null)
        {
            int nuevoId = InventarioDb.Count != 0 ? InventarioDb.Max(i => i.Id) + 1 : 1;
            InventarioDb.Add(new InventarioResponse(nuevoId, productoId, cantidadInicial, DateTime.UtcNow));
        }
        else
        {
            int index = InventarioDb.IndexOf(item);
            InventarioDb[index] = item with 
            { 
                CantidadDisponible = cantidadInicial,
                FechaActualizacion = DateTime.UtcNow 
            };
        }
    }

    public static InventarioResponse AumentarStock(int productoId, int cantidad)
    {
        var item = ObtenerPorProductoId(productoId);

        if (item is null)
        {
            int nuevoId = InventarioDb.Count != 0 ? InventarioDb.Max(i => i.Id) + 1 : 1;
            var nuevoItem = new InventarioResponse(nuevoId, productoId, cantidad, DateTime.UtcNow);
            InventarioDb.Add(nuevoItem);
            return nuevoItem;
        }

        int index = InventarioDb.IndexOf(item);
        var actualizado = item with 
        { 
            CantidadDisponible = item.CantidadDisponible + cantidad,
            FechaActualizacion = DateTime.UtcNow 
        };
        
        InventarioDb[index] = actualizado;
        return actualizado;
    }

    public static (bool exito, InventarioResponse? inventario, string? mensajeError) DisminuirStock(int productoId, int cantidad)
    {
        var item = ObtenerPorProductoId(productoId);

        if (item is null)
        {
            return (false, null, $"No se encontró registro de inventario para el producto ID: {productoId}");
        }

        if (item.CantidadDisponible < cantidad)
        {
            return (false, null, $"Stock insuficiente. Disponible: {item.CantidadDisponible}, Requerido: {cantidad}");
        }

        int index = InventarioDb.IndexOf(item);
        var actualizado = item with 
        { 
            CantidadDisponible = item.CantidadDisponible - cantidad,
            FechaActualizacion = DateTime.UtcNow 
        };

        InventarioDb[index] = actualizado;
        return (true, actualizado, null);
    }
}