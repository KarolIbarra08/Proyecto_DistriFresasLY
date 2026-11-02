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
        : base(id, "Insumo")
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

        if (string.IsNullOrWhiteSpace(unidadMedida))
            return Result.Failure<ProductoInsumo>(Error.Validation("ProductoInsumo.UnidadMedidaVacia", "La unidad de medida es obligatoria."));

        if (string.IsNullOrWhiteSpace(descripcion))
            return Result.Failure<ProductoInsumo>(Error.Validation("ProductoInsumo.DescripcionVacia", "La descripción es obligatoria."));

        return Result.Success(new ProductoInsumo(
            id, 
            nombre.Trim(), 
            tipoInsumo.Trim(), 
            unidadMedida.Trim(), 
            descripcion.Trim()));
    }

    public Result ActualizarDatos(string? nombre, string? tipoInsumo, string? unidadMedida, string? descripcion)
    {
        var nombreFinal = nombre ?? Nombre;
        var tipoInsumoFinal = tipoInsumo ?? TipoInsumo;
        var unidadMedidaFinal = unidadMedida ?? UnidadMedida;
        var descripcionFinal = descripcion ?? Descripcion;

        if (string.IsNullOrWhiteSpace(nombreFinal))
            return Result.Failure(Error.Validation("ProductoInsumo.NombreVacio", "El nombre es obligatorio."));

        if (string.IsNullOrWhiteSpace(tipoInsumoFinal))
            return Result.Failure(Error.Validation("ProductoInsumo.TipoVacio", "El tipo de insumo es obligatorio."));

        if (string.IsNullOrWhiteSpace(unidadMedidaFinal))
            return Result.Failure(Error.Validation("ProductoInsumo.UnidadMedidaVacia", "La unidad de medida es obligatoria."));

        if (string.IsNullOrWhiteSpace(descripcionFinal))
            return Result.Failure(Error.Validation("ProductoInsumo.DescripcionVacia", "La descripción es obligatoria."));

        Nombre = nombreFinal.Trim();
        TipoInsumo = tipoInsumoFinal.Trim();
        UnidadMedida = unidadMedidaFinal.Trim();
        Descripcion = descripcionFinal.Trim();

        return Result.Success();
    }
}