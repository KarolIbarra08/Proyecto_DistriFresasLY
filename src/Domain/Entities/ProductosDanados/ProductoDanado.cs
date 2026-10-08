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
    public double Valor { get; private set; }
    public DateTime FechaProductoDanado { get; private set; }
    public string Motivo { get; private set; }

    // Usamos el namespace completo para evitar la confusión del compilador
    public DistriFresasLY.Domain.ValueObjects.Descuento? Descuento { get; private set; }

    private ProductoDanado(
        int id,
        int productoId,
        string tipoProducto,
        int cantidad,
        string identificacion,
        double valor,
        DateTime fechaProductoDanado,
        string motivo,
        DistriFresasLY.Domain.ValueObjects.Descuento? descuento = null)
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
        DistriFresasLY.Domain.ValueObjects.Descuento? descuento = null)
    {
        if (cantidad <= 0)
            return Result.Failure<ProductoDanado>(Error.Validation("ProductoDanado.CantidadInvalida", "La cantidad debe ser mayor a cero."));

        if (valor < 0)
            return Result.Failure<ProductoDanado>(Error.Validation("ProductoDanado.ValorInvalido", "El valor no puede ser negativo."));

        if (string.IsNullOrWhiteSpace(identificacion))
            return Result.Failure<ProductoDanado>(Error.Validation("ProductoDanado.IdentificacionRequerida", "La identificación es requerida."));

        var productoDanado = new ProductoDanado(
            id,
            productoId,
            tipoProducto.Trim(),
            cantidad,
            identificacion.Trim(),
            valor,
            fechaProductoDanado,
            motivo.Trim(),
            descuento);

        return Result.Success(productoDanado);
    }

    public Result CalcularDescuento()
    {
        // Se llama directamente al Value Object
        var descuentoResult = DistriFresasLY.Domain.ValueObjects.Descuento.CalcularValor(Cantidad, Valor, Motivo);

        if (descuentoResult.IsFailure)
            return Result.Failure(descuentoResult.Error);

        Descuento = descuentoResult.Value;
        return Result.Success();
    }
}