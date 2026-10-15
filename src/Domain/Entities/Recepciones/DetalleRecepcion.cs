using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Recepciones;

public class DetalleRecepcion : Entity
{
    public int IdRecepcion { get; private set; }
    public int IdProducto { get; private set; }
    public double Cantidad { get; private set; }
    public double PrecioUnitario { get; private set; }
    public DateTime FechaIngreso { get; private set; }
    

    public double Subtotal => Cantidad * PrecioUnitario;

    private DetalleRecepcion(
        int id,
        int idRecepcion,
        int idProducto,
        int cantidad,
        double precioUnitario,
        DateTime fechaIngreso)
        : base(id)
    {
        IdRecepcion = idRecepcion;
        IdProducto = idProducto;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        FechaIngreso = fechaIngreso;
    }

    public static Result<DetalleRecepcion> Create(
        int idProducto,
        int cantidad,
        double precioUnitario,
        int idRecepcion = 0,
        int id = 0,
        DateTime? fechaIngreso = null)
    {
        if (idProducto <= 0)
        {
            return Result.Failure<DetalleRecepcion>(
                Error.Validation("DetalleRecepcion.ProductoObligatorio", "El producto es obligatorio."));
        }

        if (cantidad <= 0)
        {
            return Result.Failure<DetalleRecepcion>(
                Error.Validation("DetalleRecepcion.CantidadInvalida", "La cantidad recibida debe ser mayor a cero."));
        }

        if (precioUnitario < 0)
        {
            return Result.Failure<DetalleRecepcion>(
                Error.Validation("DetalleRecepcion.PrecioInvalido", "El precio unitario no puede ser negativo."));
        }

        return Result.Success(
            new DetalleRecepcion(
                id,
                idRecepcion,
                idProducto,
                cantidad,
                precioUnitario,
                fechaIngreso ?? DateTime.UtcNow));
    }

    public Result Actualizar(double cantidad, double precioUnitario)
    {
        if (cantidad <= 0)
        {
            return Result.Failure(
                Error.Validation("DetalleRecepcion.CantidadInvalida", "La cantidad recibida debe ser mayor a cero."));
        }

        if (precioUnitario < 0)
        {
            return Result.Failure(
                Error.Validation("DetalleRecepcion.PrecioInvalido", "El precio unitario no puede ser negativo."));
        }

        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;

        return Result.Success();
    }

    internal void AsignarRecepcion(int idRecepcion)
    {
        IdRecepcion = idRecepcion;
    }
}