using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.ProductosDanados;

public class ProductoDanado
{
    public int Id { get; private set; }

    public int ProductoId { get; private set; }

    public string TipoProducto { get; private set; }

    public int Cantidad { get; private set; }

    public string Identificacion { get; private set; }

    // Valor unitario del producto dañado
    public double Valor { get; private set; }

    public DateTime FechaProductoDanado { get; private set; }

    public string Motivo { get; private set; }

    public Descuento? Descuento { get; private set; }

    private ProductoDanado(
        int id,
        int productoId,
        string tipoProducto,
        int cantidad,
        string identificacion,
        double valor,
        DateTime fechaProductoDanado,
        string motivo,
        Descuento? descuento = null)
    {
        Id = id;
        ProductoId = productoId;
        TipoProducto = tipoProducto;
        Cantidad = cantidad;
        Identificacion = identificacion;
        Valor = valor;
        FechaProductoDanado = fechaProductoDanado;
        Motivo = motivo;
        Descuento = descuento;
    }

    public static Result<ProductoDanado> Create(
        int productoId,
        string tipoProducto,
        int cantidad,
        string identificacion,
        double valor,
        DateTime fechaProductoDanado,
        string motivo,
        int id = 0,
        Descuento? descuento = null)
    {
        if (productoId <= 0)
        {
            return Result.Failure<ProductoDanado>(
                Error.Validation(
                    "ProductoDanado.ProductoInvalido",
                    "El producto es obligatorio."));
        }

        if (string.IsNullOrWhiteSpace(tipoProducto))
        {
            return Result.Failure<ProductoDanado>(
                Error.Validation(
                    "ProductoDanado.TipoProductoRequerido",
                    "El tipo de producto es obligatorio."));
        }

        if (cantidad <= 0)
        {
            return Result.Failure<ProductoDanado>(
                Error.Validation(
                    "ProductoDanado.CantidadInvalida",
                    "La cantidad debe ser mayor que cero."));
        }

        if (valor <= 0)
        {
            return Result.Failure<ProductoDanado>(
                Error.Validation(
                    "ProductoDanado.ValorInvalido",
                    "El valor debe ser mayor que cero."));
        }

        if (string.IsNullOrWhiteSpace(identificacion))
        {
            return Result.Failure<ProductoDanado>(
                Error.Validation(
                    "ProductoDanado.IdentificacionRequerida",
                    "La identificación es requerida."));
        }

        if (string.IsNullOrWhiteSpace(motivo))
        {
            return Result.Failure<ProductoDanado>(
                Error.Validation(
                    "ProductoDanado.MotivoRequerido",
                    "El motivo es obligatorio."));
        }

        return Result.Success(
            new ProductoDanado(
                id,
                productoId,
                tipoProducto.Trim(),
                cantidad,
                identificacion.Trim(),
                valor,
                fechaProductoDanado,
                motivo.Trim(),
                descuento));
    }

    public Result CalcularDescuento()
    {
        var descuentoResult =
            Descuento.CalcularValor(
                Cantidad,
                Valor,
                Motivo);

        if (descuentoResult.IsFailure)
        {
            return Result.Failure(
                descuentoResult.Error);
        }

        Descuento = descuentoResult.Value;

        return Result.Success();
    }
}