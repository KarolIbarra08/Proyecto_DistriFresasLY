using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.Productos;

public class ProductoFresa : Producto
{
    public Calibre Calibre { get; private set; }
    public string Calidad { get; private set; }
    public Peso Peso { get; private set; }
    public string Descripcion { get; private set; }

    private ProductoFresa(
        int id,
        Calibre calibre,
        string calidad,
        Peso peso,
        string descripcion)
        : base(id, "Fresa")
    {
        Calibre = calibre;
        Calidad = calidad;
        Peso = peso;
        Descripcion = descripcion;
    }

    public static Result<ProductoFresa> Create(
        int calibre,
        string calidad,
        double peso,
        string descripcion,
        Func<int, bool> existeCalibre, 
        int id = 0)
    {

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            return Result.Failure<ProductoFresa>(
                Error.Validation(
                    "ProductoFresa.DescripcionVacia",
                    "La descripción es obligatoria."));
        }

        if (string.IsNullOrWhiteSpace(calidad))
        {
            return Result.Failure<ProductoFresa>(
                Error.Validation(
                    "ProductoFresa.CalidadVacia",
                    "La calidad no puede estar vacía."));
        }


        if (existeCalibre(calibre))
        {
            return Result.Failure<ProductoFresa>(
                Error.Conflict(
                    "ProductoFresa.CalibreDuplicado",
                    $"El calibre {calibre} ya se encuentra registrado."));
        }

   
        var calibreResult = Calibre.Create(calibre);
        if (calibreResult.IsFailure)
        {
            return Result.Failure<ProductoFresa>(calibreResult.Error);
        }

        var pesoResult = Peso.Create(peso);
        if (pesoResult.IsFailure)
        {
            return Result.Failure<ProductoFresa>(pesoResult.Error);
        }

        return Result.Success(
            new ProductoFresa(
                id,
                calibreResult.Value,
                calidad.Trim(),
                pesoResult.Value,
                descripcion.Trim()));
    }

    public Result Actualizar(
        int calibre,
        string calidad,
        double peso,
        string descripcion,
        Func<int, bool> existeCalibreParaOtro) 
    {
        if (string.IsNullOrWhiteSpace(descripcion))
        {
            return Result.Failure(
                Error.Validation(
                    "ProductoFresa.DescripcionVacia",
                    "La descripción es obligatoria."));
        }

        if (string.IsNullOrWhiteSpace(calidad))
        {
            return Result.Failure(
                Error.Validation(
                    "ProductoFresa.CalidadVacia",
                    "La calidad no puede estar vacía."));
        }

  
        if (existeCalibreParaOtro(calibre))
        {
            return Result.Failure(
                Error.Conflict(
                    "ProductoFresa.CalibreDuplicado",
                    $"El calibre {calibre} ya pertenece a otra fresa registrada."));
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

        Calibre = calibreResult.Value;
        Calidad = calidad.Trim();
        Peso = pesoResult.Value;
        Descripcion = descripcion.Trim();

        return Result.Success();
    }
}