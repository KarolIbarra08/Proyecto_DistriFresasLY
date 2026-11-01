using DistriFresasLY.Domain.Entities.Ventas;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

public sealed class VentaRegistro
{
    public int Id_Venta { get; set; }
    public string Fecha_Venta { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int? Id_Cliente { get; set; }

    public List<DetalleVentaRegistro> Detalles { get; set; } = [];

    public Venta ToEntidad()
    {
        var fecha = DateTime.TryParse(Fecha_Venta, out var parsedDate) ? parsedDate : DateTime.UtcNow;

        var detallesEntidades = Detalles
            .Select(d => d.ToEntidad())
            .ToList();

        return Venta.Rehidratar(
            id: Id_Venta,
            fechaVenta: fecha,
            estado: Estado,
            clienteId: Id_Cliente ?? 0,
            detalles: detallesEntidades);
    }
}

public sealed class DetalleVentaRegistro
{
    public int Id_Detalle_Venta { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio_Unitario { get; set; }
    public decimal Subtotal { get; set; }
    public int Id_Producto { get; set; }
    public int Id_Venta { get; set; }

    public DetalleVenta ToEntidad() => DetalleVenta.Rehidratar(
        id: Id_Detalle_Venta,
        productoId: Id_Producto,
        cantidad: Cantidad,
        precioUnitario: (double)Precio_Unitario);
}
