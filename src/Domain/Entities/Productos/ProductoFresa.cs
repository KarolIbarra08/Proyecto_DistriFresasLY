using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Productos;

public class ProductoFresa
{
    public int Id { get; private set; }
    public string TipoProducto { get; private set; } = "Fresa";
    public int Calibre { get; private set; }
    public string Calidad { get; private set; }
    public double Peso { get; private set; }
    public decimal PrecioCompra { get; private set; }
    public DateTime FechaIngreso { get; private set; }
    public string Descripcion { get; private set; }

    private ProductoFresa(
        int calibre,
        string calidad,
        double peso,
        decimal precioCompra,
        string descripcion,
        int id,
        DateTime fechaIngreso)
    {
        Id = id;
        Calibre = calibre;
        Calidad = calidad;
        Peso = peso;
        PrecioCompra = precioCompra;
        Descripcion = descripcion;
        FechaIngreso = fechaIngreso;
    }

    public static Result<ProductoFresa> Create(
        int calibre,
        string calidad,
        double peso,
        decimal precioCompra,
        string descripcion,
        int id = 0,
        DateTime? fechaIngreso = null)
    {
        if (string.IsNullOrWhiteSpace(calidad))
            return Result.Failure<ProductoFresa>(Error.Validation("ProductoFresa.CalidadVacia", "La calidad no puede estar vacía."));

        return Result.Success(new ProductoFresa(
            calibre,
            calidad.Trim(),
            peso,
            precioCompra,
            descripcion.Trim(),
            id,
            fechaIngreso ?? DateTime.UtcNow
        ));
    }
}