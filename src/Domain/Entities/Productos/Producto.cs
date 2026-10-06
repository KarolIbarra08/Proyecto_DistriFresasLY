using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Productos;

public abstract class Producto : Entity
{
    public string TipoProducto { get; protected set; }

    protected Producto(int id, string tipoProducto) : base(id)
    {
        TipoProducto = tipoProducto;
    }
}