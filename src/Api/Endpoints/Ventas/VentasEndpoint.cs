using DistriFresasLY.Domain.Entities.Ventas;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public static class VentaDataStore
{
    public static readonly List<Venta> VentasDb = CrearSeed();

    private static List<Venta> CrearSeed()
    {
        var detalle = DetalleVenta.Create(101, 10, 15000m).Value;
        var venta = Venta.Create([detalle], clienteId: 1, id: 1).Value;
        venta.ActualizarEstado("Completada");
        return [venta];
    }
}