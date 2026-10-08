using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Productos;

public abstract class Producto : Entity
{
    public string TipoProducto { get; protected set; }

    protected Producto(
        int id,
        string tipoProducto)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(tipoProducto))
        {
            throw new ArgumentException(
                "El tipo de producto es obligatorio.");
        }

        TipoProducto = tipoProducto.Trim();
    }
}