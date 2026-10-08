using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Productos;

public class ProductoInsumo
{
    public int Id { get; private set; }
    public string TipoProducto { get; private set; } = "Insumo";
    public string Nombre { get; private set; }
    public string Tipo { get; private set; }
    public string UnidadMedida { get; private set; }
    public string Descripcion { get; private set; }
    public DateTime FechaIngreso { get; private set; }

    private ProductoInsumo(
        string nombre, 
        string tipo, 
        string unidadMedida, 
        string descripcion, 
        int id, 
        DateTime fechaIngreso)
    {
        Id = id;
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
            return Result.Failure<ProductoInsumo>(Error.Validation("ProductoInsumo.NombreVacio", "El nombre del insumo no puede estar vacío."));

        return Result.Success(new ProductoInsumo(
            nombre.Trim(),
            tipo.Trim(),
            unidadMedida.Trim(),
            descripcion.Trim(),
            id,
            fechaIngreso ?? DateTime.UtcNow
        ));
    }
}