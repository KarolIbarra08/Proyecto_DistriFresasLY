using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.Productos;

public class ProductoFresa : Producto
{
    public Calibre Calibre { get; private set; }

    public string Calidad { get; private set; }

    public Peso Peso { get; private set; }

    public PrecioCompra PrecioCompra { get; private set; }

    public DateTime FechaIngreso { get; private set; }

    public string Descripcion { get; private set; }


    private ProductoFresa(
        int id,
        Calibre calibre,
        string calidad,
        Peso peso,
        PrecioCompra precioCompra,
        DateTime fechaIngreso,
        string descripcion)
        : base(id, "Fresa")
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
        int id = 0,
        DateTime? fechaIngreso = null)
    {
        if (string.IsNullOrWhiteSpace(calidad))
        {
            return Result.Failure<ProductoFresa>(
                Error.Validation(
                    "ProductoFresa.CalidadVacia",
                    "La calidad no puede estar vacía."));
        }

        var calibreResult = Calibre.Create(calibre);

        if (calibreResult.IsFailure)
        {
            return Result.Failure<ProductoFresa>(
                calibreResult.Error);
        }

        var pesoResult = Peso.Create(peso);

        if (pesoResult.IsFailure)
        {
            return Result.Failure<ProductoFresa>(
                pesoResult.Error);
        }

        var precioResult = PrecioCompra.Create(precioCompra);

        if (precioResult.IsFailure)
        {
            return Result.Failure<ProductoFresa>(
                precioResult.Error);
        }

        return Result.Success(
            new ProductoFresa(
                id,
                calibreResult.Value,
                calidad.Trim(),
                pesoResult.Value,
                precioResult.Value,
                fechaIngreso ?? DateTime.UtcNow,
                descripcion?.Trim() ?? string.Empty));
    }


    public Result Actualizar(
        int calibre,
        string calidad,
        double peso,
        decimal precioCompra,
        string descripcion)
    {
        if (string.IsNullOrWhiteSpace(calidad))
        {
            return Result.Failure(
                Error.Validation(
                    "ProductoFresa.CalidadVacia",
                    "La calidad no puede estar vacía."));
        }

        var calibreResult = Calibre.Create(calibre);

        if (calibreResult.IsFailure)
        {
            return Result.Failure(calibreResult.Error);
        }

        var pesoResult = Peso.Create(peso);

        if (pesoResult.IsFailure)
        {
            return Result.Failure(pesoResult.Error);
        }

        var precioResult = PrecioCompra.Create(precioCompra);

        if (precioResult.IsFailure)
        {
            return Result.Failure(precioResult.Error);
        }

        Calibre = calibreResult.Value;
        Calidad = calidad.Trim();
        Peso = pesoResult.Value;
        PrecioCompra = precioResult.Value;
        Descripcion = descripcion?.Trim() ?? string.Empty;

        return Result.Success();
    }
}