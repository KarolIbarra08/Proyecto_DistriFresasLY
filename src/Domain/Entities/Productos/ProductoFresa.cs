using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Productos;

public sealed class ProductoFresa : Producto
{
    public int Calibre { get; private set; }
    public string Calidad { get; private set; }
    public double Peso { get; private set; }
    public decimal PrecioCompra { get; private set; }
    public DateTime FechaIngreso { get; private set; }
    public string Descripcion { get; private set; }

    private ProductoFresa(
        int id,
        int calibre,
        string calidad,
        double peso,
        decimal precioCompra,
        DateTime fechaIngreso,
        string descripcion) : base(id, "Fresa")
    {
        Calibre = calibre;
        Calidad = calidad;
        Peso = peso;
        PrecioCompra = precioCompra;
        FechaIngreso = fechaIngreso;
        Descripcion = descripcion;
    }

    public static Result<ProductoFresa> Create(
        int calibre,
        string calidad,
        double peso,
        decimal precioCompra,
        string descripcion,
        int id = 0)
    {
        if (calibre <= 0)
            return Result.Failure<ProductoFresa>(Error.Validation("ProductoFresa.CalibreInvalido", "El calibre debe ser mayor a 0."));

        if (string.IsNullOrWhiteSpace(calidad))
            return Result.Failure<ProductoFresa>(Error.Validation("ProductoFresa.CalidadRequerida", "La calidad es obligatoria."));

        if (precioCompra <= 0)
            return Result.Failure<ProductoFresa>(Error.Validation("ProductoFresa.PrecioInvalido", "El precio de compra debe ser mayor a 0."));

        var producto = new ProductoFresa(id, calibre, calidad, peso, precioCompra, DateTime.Now, descripcion);
        return Result.Success(producto);
    }
}