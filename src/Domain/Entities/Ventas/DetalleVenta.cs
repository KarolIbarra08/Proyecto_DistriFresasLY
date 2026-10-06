using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities;

public sealed class DetalleVenta : Entity
{
    public int ProductoId { get; private set; }
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;

    private DetalleVenta(int id, int productoId, int cantidad, decimal precioUnitario) : base(id)
    {
        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }

    public static Result<DetalleVenta> Create(int productoId, int cantidad, decimal precioUnitario, int id = 0)
    {
        if (productoId <= 0)
        {
            return Error.Validation(
                "DetalleVenta.ProductoInvalido",
                "El ID del producto debe ser mayor a 0.");
        }

        if (cantidad <= 0)
        {
            return Error.Validation(
                "DetalleVenta.CantidadInvalida",
                "La cantidad vendida debe ser mayor a 0.");
        }

        if (precioUnitario <= 0)
        {
            return Error.Validation(
                "DetalleVenta.PrecioInvalido",
                "El precio unitario debe ser mayor a 0.");
        }

        return new DetalleVenta(id, productoId, cantidad, precioUnitario);
    }
}