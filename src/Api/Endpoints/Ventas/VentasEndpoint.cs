using DistriFresasLY.Domain.Entities.Ventas;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public static class VentaDataStore
{
    public static readonly List<Venta> VentasDb = CrearSeed();

    private static List<Venta> CrearSeed()
    {
        var detalle = DetalleVenta.Create(101, 20, 15000.0).Value; // Cantidad 20 lbs (> 17) para cumplir Regla 1

        var stockSeed = new Dictionary<int, int> { { 101, 100 } }; 

        var venta = Venta.Create(
            detalles: [detalle], 
            clienteId: 1, 
            stockDisponiblePorProducto: stockSeed, 
            esAdministrador: true, 
            id: 1).Value;

        venta.ActualizarEstado("Completada");
        return [venta];
    }
}