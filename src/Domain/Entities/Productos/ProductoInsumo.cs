using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Productos;

public sealed class ProductoInsumo : Producto
{
    public string Nombre { get; private set; }
    public string TipoInsumo { get; private set; }
    public string UnidadMedida { get; private set; }
    public string Descripcion { get; private set; }

    private ProductoInsumo(
        int id,
        string nombre,
        string tipoInsumo,
        string unidadMedida,
        string descripcion)
        : base(id, "Insumo") // Asigna "Insumo" a TipoProducto en la clase base Producto
    {
        Nombre = nombre;
        TipoInsumo = tipoInsumo;
        UnidadMedida = unidadMedida;
        Descripcion = descripcion;
    }

    public static ProductoInsumo Rehidratar(
        int id, 
        string nombre, 
        string tipoInsumo, 
        string unidadMedida, 
        string descripcion)
    {
        return new ProductoInsumo(
            id, 
            nombre ?? string.Empty, 
            tipoInsumo ?? string.Empty, 
            unidadMedida ?? string.Empty, 
            descripcion ?? string.Empty);
    }

    public static Result<ProductoInsumo> Create(
        string nombre,
        string tipoInsumo,
        string unidadMedida,
        string descripcion,
        int id = 0)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Failure<ProductoInsumo>(Error.Validation("ProductoInsumo.NombreVacio", "El nombre es obligatorio."));

        if (string.IsNullOrWhiteSpace(tipoInsumo))
            return Result.Failure<ProductoInsumo>(Error.Validation("ProductoInsumo.TipoVacio", "El tipo de insumo es obligatorio."));

        return Result.Success(new ProductoInsumo(
            id, 
            nombre.Trim(), 
            tipoInsumo.Trim(), 
            (unidadMedida ?? string.Empty).Trim(), 
            (descripcion ?? string.Empty).Trim()));
    }

    public Result ActualizarDatos(string? nombre, string? tipoInsumo, string? unidadMedida, string? descripcion)
    {
        // 1. Si enviaron un nuevo nombre (no nulo ni vacío), actualizar
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            Nombre = nombre.Trim();
        }

        // 2. Si enviaron un nuevo tipo de insumo, actualizar
        if (!string.IsNullOrWhiteSpace(tipoInsumo))
        {
            TipoInsumo = tipoInsumo.Trim();
        }

        // 3. Si enviaron una nueva unidad de medida, actualizar
        if (!string.IsNullOrWhiteSpace(unidadMedida))
        {
            UnidadMedida = unidadMedida.Trim();
        }

        // 4. Si enviaron una nueva descripción (no nula), actualizar
        if (descripcion is not null)
        {
            Descripcion = descripcion.Trim();
        }

        return Result.Success();
    }
}