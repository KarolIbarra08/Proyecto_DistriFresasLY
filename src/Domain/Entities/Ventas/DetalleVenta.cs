namespace DistriFresasLY.Domain.Entities;

public class DetalleVenta
{
    public int ProductoId { get; private set; }
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }

    // Regla de Dominio
    public decimal Subtotal => Cantidad * PrecioUnitario;

    public DetalleVenta(int productoId, int cantidad, decimal precioUnitario)
    {
        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }
}