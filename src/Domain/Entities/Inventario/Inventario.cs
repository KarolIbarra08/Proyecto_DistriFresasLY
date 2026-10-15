using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Inventario;

public sealed class Inventario : Entity
{
    public int ProductoId { get; private set; }
    public int CantidadDisponible { get; private set; }
    public DateTime FechaActualizacion { get; private set; }
    private Inventario(int id, int productoId, int cantidadDisponible) : base(id)
    {
        ProductoId = productoId;
        CantidadDisponible = cantidadDisponible;
        FechaActualizacion = ColombiaTime.Ahora;
    }

    public static Result<Inventario> Create(int productoId, int cantidadDisponible, int id = 0)
    {
        if (productoId <= 0)
        {
            return Error.Validation(
                "Inventario.ProductoInvalido",
                "El ID del producto no es valido.");
        }

        if (cantidadDisponible < 0)
        {
            return Error.Validation(
                "Inventario.CantidadInvalida",
                "La cantidad disponible no puede ser negativa.");
        }

        return new Inventario(id, productoId, cantidadDisponible);
    }

    public Result Ajustar(int nuevaCantidad)
    {
        if (nuevaCantidad < 0)
        {
            return Result.Failure(Error.Validation(
                "Inventario.CantidadInvalida",
                "La cantidad disponible no puede ser negativa."));
        }

        CantidadDisponible = nuevaCantidad;
        FechaActualizacion = ColombiaTime.Ahora;

        return Result.Success();
    }

    public Result Aumentar(int cantidad)
    {
        if (cantidad <= 0)
        {
            return Result.Failure(Error.Validation(
                "Inventario.CantidadInvalida",
                "La cantidad a aumentar debe ser mayor a 0."));
        }

        CantidadDisponible += cantidad;
        FechaActualizacion = ColombiaTime.Ahora;

        return Result.Success();
    }

    public bool PuedeDisminuir(int cantidad)
    {
        return cantidad > 0 && CantidadDisponible >= cantidad;
    }

    public Result Disminuir(int cantidad)
    {
        if (cantidad <= 0)
        {
            return Result.Failure(Error.Validation(
                "Inventario.CantidadInvalida",
                "La cantidad a disminuir debe ser mayor a 0."));
        }

        if (CantidadDisponible < cantidad)
        {
            return Result.Failure(Error.Validation(
                "Inventario.StockInsuficiente",
                "No hay suficiente stock disponible."));
        }

        CantidadDisponible -= cantidad;
        FechaActualizacion = ColombiaTime.Ahora;

        return Result.Success();
    }
}