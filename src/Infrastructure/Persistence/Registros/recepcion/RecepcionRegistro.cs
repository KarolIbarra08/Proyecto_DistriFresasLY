using DistriFresasLY.Domain.Entities.Recepciones;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

public sealed class RecepcionRegistro
{
    public int Id_Recepcion { get; set; }
    public string Fecha_Recepcion { get; set; } = string.Empty;
    public int? Id_Pedido { get; set; }
    public int Procesada_En_Inventario { get; set; }

    public List<DetalleRecepcionRegistro> Detalles { get; set; } = [];

    public Recepcion ToEntidad()
    {
        var fecha = DateTime.TryParse(Fecha_Recepcion, out var parsedDate) ? parsedDate : DateTime.UtcNow;

        var detallesEntidades = Detalles
            .Select(d => d.ToEntidad())
            .ToList();

        return Recepcion.Rehidratar(
            id: Id_Recepcion,
            pedidoId: Id_Pedido,
            fechaRecepcion: fecha,
            procesadaEnInventario: Procesada_En_Inventario == 1,
            detalles: detallesEntidades);
    }
}

public sealed class DetalleRecepcionRegistro
{
    public int Id_Detalle_Recepcion { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio_Unitario { get; set; }
    public string Fecha_Ingreso { get; set; } = string.Empty;
    public int Id_Recepcion { get; set; }
    public int Id_Producto { get; set; }

    public DetalleRecepcion ToEntidad()
    {
        var fecha = DateTime.TryParse(Fecha_Ingreso, out var parsedDate) ? parsedDate : DateTime.UtcNow;

        return DetalleRecepcion.Rehidratar(
            id: Id_Detalle_Recepcion,
            idRecepcion: Id_Recepcion,
            idProducto: Id_Producto,
            cantidad: Cantidad,
            precioUnitario: (double)Precio_Unitario,
            fechaIngreso: fecha);
    }
}
