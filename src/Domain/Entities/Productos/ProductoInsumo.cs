using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Productos;

public sealed class ProductoInsumo : Producto
{
    public string Nombre { get; private set; }
    public string Tipo { get; private set; }
    public string UnidadMedida { get; private set; }
    public string Descripcion { get; private set; }

    private ProductoInsumo(
        int id,
        string nombre,
        string tipo,
        string unidadMedida,
        string descripcion) : base(id, "Insumo")
    {
        Nombre = nombre;
        Tipo = tipo;
        UnidadMedida = unidadMedida;
        Descripcion = descripcion;
    }

    public static Result<ProductoInsumo> Create(
        string nombre,
        string tipo,
        string unidadMedida,
        string descripcion,
        int id = 0)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Failure<ProductoInsumo>(Error.Validation("ProductoInsumo.NombreRequerido", "El nombre es obligatorio."));

        if (string.IsNullOrWhiteSpace(unidadMedida))
            return Result.Failure<ProductoInsumo>(Error.Validation("ProductoInsumo.UnidadMedidaRequerida", "La unidad de medida es obligatoria."));

        var producto = new ProductoInsumo(id, nombre, tipo, unidadMedida, descripcion);
        return Result.Success(producto);
    }
}