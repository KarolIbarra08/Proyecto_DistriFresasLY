namespace DistriFresasLY.Domain.Entities;

public class Inventario
{
    public int Id { get; private set; }
    public int ProductoId { get; private set; }
    public int CantidadDisponible { get; private set; }
    public DateTime FechaActualizacion { get; private set; }

    public Inventario(int id, int productoId, int cantidadDisponible)
    {
        Id = id;
        ProductoId = productoId;
        CantidadDisponible = cantidadDisponible;
        FechaActualizacion = DateTime.Now;
    }

    public void Ajustar(int nuevaCantidad)
    {
        CantidadDisponible = nuevaCantidad;
        FechaActualizacion = DateTime.Now;
    }

    public void Aumentar(int cantidad)
    {
        CantidadDisponible += cantidad;
        FechaActualizacion = DateTime.Now;
    }

    public bool PuedeDisminuir(int cantidad)
    {
        return CantidadDisponible >= cantidad;
    }

    public void Disminuir(int cantidad)
    {
        CantidadDisponible -= cantidad;
        FechaActualizacion = DateTime.Now;
    }
}