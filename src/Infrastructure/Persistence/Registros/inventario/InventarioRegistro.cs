using DistriFresasLY.Domain.Entities.Inventario;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

public sealed class InventarioRegistro
{
    public int Id_Inventario { get; set; }
    public int Id_Producto { get; set; }
    public int Cantidad_Disponible { get; set; }
    public string Fecha_Actualizacion { get; set; } = string.Empty;

    public Inventario ToEntidad()
    {
        var fecha = DateTime.TryParse(Fecha_Actualizacion, out var parsedDate) ? parsedDate : DateTime.UtcNow;
        return Inventario.Rehidratar(Id_Inventario, Id_Producto, Cantidad_Disponible, fecha);
    }
}
