using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.Ventas;

public sealed class DetalleVenta : Entity
{
    public int ProductoId { get; private set; }

    public Cantidad Cantidad { get; private set; }

    public double PrecioUnitario { get; private set; }

    public double Subtotal => Cantidad * PrecioUnitario;

    private DetalleVenta(int id, int productoId, Cantidad cantidad, double precioUnitario) : base(id)
    {
        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }

    public static Result<DetalleVenta> Create(int productoId, int cantidad, double precioUnitario, int id = 0)
    {
        if (productoId <= 0)
        {
            return Error.Validation(
                "DetalleVenta.ProductoInvalido",
                "El ID del producto debe ser mayor a 0.");
        }

        var cantidadResult = Cantidad.Create(cantidad);
        if (cantidadResult.IsFailure)
        {
            return Result.Failure<DetalleVenta>(cantidadResult.Error);
        }

        if (precioUnitario <= 0)
        {
            return Error.Validation(
                "DetalleVenta.PrecioInvalido",
                "El precio unitario debe ser mayor a 0.");
        }

        return new DetalleVenta(id, productoId, cantidadResult.Value, precioUnitario);
    }

    public static DetalleVenta Rehidratar(int id, int productoId, int cantidad, double precioUnitario)
    {
        var cantidadResult = Cantidad.Create(cantidad <= 0 ? 1 : cantidad);
        var cantidadFinal = cantidadResult.IsSuccess ? cantidadResult.Value : Cantidad.Create(1).Value;

        return new DetalleVenta(id, productoId, cantidadFinal, precioUnitario);
    }
}