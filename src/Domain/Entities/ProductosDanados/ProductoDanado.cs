using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.ProductosDanados;

public sealed class ProductoDanado : Entity
{
    public int ProductoId { get; private set; }

    public string TipoProducto { get; private set; }

    public Cantidad Cantidad { get; private set; }

    public string Identificacion { get; private set; }

    // Valor unitario del producto dañado
    public decimal Valor { get; private set; }

    public DateTime FechaProductoDanado { get; private set; }

    public string Motivo { get; private set; }

    public Descuento? Descuento { get; private set; }

    private ProductoDanado(
        int id,
        int productoId,
        string tipoProducto,
        Cantidad cantidad,
        string identificacion,
        decimal valor,
        DateTime fechaProductoDanado,
        string motivo,
        Descuento? descuento = null)
        : base(id)
    {
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
        decimal valor,
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

        var cantidadResult = Cantidad.Create(cantidad);

        if (cantidadResult.IsFailure)
        {
            return Result.Failure<ProductoDanado>(cantidadResult.Error);
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
                cantidadResult.Value,
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