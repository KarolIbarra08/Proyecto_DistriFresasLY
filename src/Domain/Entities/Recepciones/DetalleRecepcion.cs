using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.Recepciones;

public class DetalleRecepcion : Entity
{
    public int IdRecepcion { get; private set; }
    public int IdProducto { get; private set; }


    public Cantidad Cantidad { get; private set; }

    public double UnidadMedida { get; private set; }
    public string TipoMedida { get; private set; }
    public decimal PrecioUnitario { get; private set; }

    public decimal Subtotal =>
        Cantidad * PrecioUnitario;

    private DetalleRecepcion(
        int id,
        int idRecepcion,
        int idProducto,
        Cantidad cantidad,
        double unidadMedida,
        string tipoMedida,
        decimal precioUnitario)
        : base(id)
    {
        IdRecepcion = idRecepcion;
        IdProducto = idProducto;
        Cantidad = cantidad;
        UnidadMedida = unidadMedida;
        TipoMedida = tipoMedida;
        PrecioUnitario = precioUnitario;
    }

    public static Result<DetalleRecepcion> Create(
        int idRecepcion,
        int idProducto,
        int cantidad,
        double unidadMedida,
        string tipoMedida,
        decimal precioUnitario,
        int id = 0)
    {
        if (idRecepcion <= 0)
        {
            return Result.Failure<DetalleRecepcion>(
                Error.Validation(
                    "DetalleRecepcion.RecepcionInvalida",
                    "La recepción es obligatoria."));
        }

        if (idProducto <= 0)
        {
            return Result.Failure<DetalleRecepcion>(
                Error.Validation(
                    "DetalleRecepcion.ProductoInvalido",
                    "El producto es obligatorio."));
        }

        var cantidadResult = Cantidad.Create(cantidad);

        if (cantidadResult.IsFailure)
        {
            return Result.Failure<DetalleRecepcion>(cantidadResult.Error);
        }

        if (unidadMedida <= 0)
        {
            return Result.Failure<DetalleRecepcion>(
                Error.Validation(
                    "DetalleRecepcion.UnidadMedidaInvalida",
                    "La unidad de medida debe ser mayor que cero."));
        }

        if (string.IsNullOrWhiteSpace(tipoMedida))
        {
            return Result.Failure<DetalleRecepcion>(
                Error.Validation(
                    "DetalleRecepcion.TipoMedidaVacio",
                    "El tipo de medida es obligatorio."));
        }

        if (precioUnitario <= 0)
        {
            return Result.Failure<DetalleRecepcion>(
                Error.Validation(
                    "DetalleRecepcion.PrecioUnitarioInvalido",
                    "El precio unitario debe ser mayor que cero."));
        }

        return Result.Success(
            new DetalleRecepcion(
                id,
                idRecepcion,
                idProducto,
                cantidadResult.Value,
                unidadMedida,
                tipoMedida.Trim(),
                precioUnitario));
    }

    public Result Actualizar(
        int cantidad,
        double unidadMedida,
        string tipoMedida,
        decimal precioUnitario)
    {
        var cantidadResult = Cantidad.Create(cantidad);

        if (cantidadResult.IsFailure)
        {
            return Result.Failure(cantidadResult.Error);
        }

        if (unidadMedida <= 0)
        {
            return Result.Failure(
                Error.Validation(
                    "DetalleRecepcion.UnidadMedidaInvalida",
                    "La unidad de medida debe ser mayor que cero."));
        }

        if (string.IsNullOrWhiteSpace(tipoMedida))
        {
            return Result.Failure(
                Error.Validation(
                    "DetalleRecepcion.TipoMedidaVacio",
                    "El tipo de medida es obligatorio."));
        }

        if (precioUnitario <= 0)
        {
            return Result.Failure(
                Error.Validation(
                    "DetalleRecepcion.PrecioUnitarioInvalido",
                    "El precio unitario debe ser mayor que cero."));
        }

        Cantidad = cantidadResult.Value;
        UnidadMedida = unidadMedida;
        TipoMedida = tipoMedida.Trim();
        PrecioUnitario = precioUnitario;

        return Result.Success();
    }
}