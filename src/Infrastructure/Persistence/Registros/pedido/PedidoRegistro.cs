using DistriFresasLY.Domain.Entities.Pedidos;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

public sealed class PedidoRegistro
{
    public int Id_Pedido { get; set; }
    public string Fecha_Pedido { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Id_Proveedor { get; set; }

    public List<DetallePedidoRegistro> Detalles { get; set; } = new();

    public Pedido ToEntidad()
    {
        var fecha = DateTime.TryParse(Fecha_Pedido, out var parsedDate) 
            ? parsedDate 
            : DateTime.UtcNow;

        var detallesEntidades = Detalles.Select(d => d.ToEntidad()).ToList();

        var result = Pedido.Create(
            proveedorId: Id_Proveedor,
            detalles: detallesEntidades,
            fechaPedido: fecha,
            estado: Estado,
            id: Id_Pedido);

        return result.Value;
    }
}

public sealed class DetallePedidoRegistro
{
    public int Id_Detalle_Pedido { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal Precio_Unitario { get; set; }
    public int Id_Pedido { get; set; }
    public int Id_Producto { get; set; }

    public DetallePedido ToEntidad()
    {
        var result = DetallePedido.Create(
            pedidoId: Id_Pedido,
            descripcion: Descripcion,
            cantidad: Cantidad,
            valorUnitario: (double)Precio_Unitario,
            id: Id_Detalle_Pedido);

        return result.Value;
    }
}