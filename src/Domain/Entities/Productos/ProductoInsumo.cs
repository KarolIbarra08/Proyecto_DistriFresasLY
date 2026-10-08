using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Productos;

public class ProductoInsumo : Producto
{
    public string Nombre { get; private set; }

    public string Tipo { get; private set; }

    public string UnidadMedida { get; private set; }

    public string Descripcion { get; private set; }

    public DateTime FechaIngreso { get; private set; }


    private ProductoInsumo(
        int id,
        string nombre,
        string tipo,
        string unidadMedida,
        string descripcion,
        DateTime fechaIngreso)
        : base(id, "Insumo")
    {
        Nombre = nombre;
        Tipo = tipo;
        UnidadMedida = unidadMedida;
        Descripcion = descripcion;
        FechaIngreso = fechaIngreso;
    }


    public static Result<ProductoInsumo> Create(
        string nombre,
        string tipo,
        string unidadMedida,
        string descripcion,
        int id = 0,
        DateTime? fechaIngreso = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Failure<ProductoInsumo>(
                Error.Validation(
                    "ProductoInsumo.NombreVacio",
                    "El nombre del insumo no puede estar vacío."));
        }

        if (string.IsNullOrWhiteSpace(tipo))
        {
            return Result.Failure<ProductoInsumo>(
                Error.Validation(
                    "ProductoInsumo.TipoVacio",
                    "El tipo de insumo es obligatorio."));
        }

        if (string.IsNullOrWhiteSpace(unidadMedida))
        {
            return Result.Failure<ProductoInsumo>(
                Error.Validation(
                    "ProductoInsumo.UnidadMedidaVacia",
                    "La unidad de medida es obligatoria."));
        }

        return Result.Success(
            new ProductoInsumo(
                id,
                nombre.Trim(),
                tipo.Trim(),
                unidadMedida.Trim(),
                descripcion?.Trim() ?? string.Empty,
                fechaIngreso ?? DateTime.UtcNow));
    }


    public Result Actualizar(
        string nombre,
        string tipo,
        string unidadMedida,
        string descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Failure(
                Error.Validation(
                    "ProductoInsumo.NombreVacio",
                    "El nombre del insumo no puede estar vacío."));
        }

        if (string.IsNullOrWhiteSpace(tipo))
        {
            return Result.Failure(
                Error.Validation(
                    "ProductoInsumo.TipoVacio",
                    "El tipo de insumo es obligatorio."));
        }

        if (string.IsNullOrWhiteSpace(unidadMedida))
        {
            return Result.Failure(
                Error.Validation(
                    "ProductoInsumo.UnidadMedidaVacia",
                    "La unidad de medida es obligatoria."));
        }

        Nombre = nombre.Trim();
        Tipo = tipo.Trim();
        UnidadMedida = unidadMedida.Trim();
        Descripcion = descripcion?.Trim() ?? string.Empty;

        return Result.Success();
    }
}