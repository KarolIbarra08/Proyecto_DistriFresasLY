using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

public sealed class PagoClienteRegistro
{
    public int Id_Pago_Cliente { get; set; }
    public decimal Valor_Total { get; set; }
    public string Medio_Pago { get; set; } = string.Empty;
    public string Fecha_Pago { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int Id_Cliente { get; set; }
    public int Id_Venta { get; set; }

    public PagoCliente ToEntidad()
    {
        var fecha = DateTime.TryParse(Fecha_Pago, out var parsedDate) ? parsedDate : DateTime.UtcNow;
        return PagoCliente.Rehidratar(Id_Pago_Cliente, Id_Cliente, Id_Venta, (double)Valor_Total, Medio_Pago, fecha, Estado);
    }
}

public sealed class PagoProveedorRegistro
{
    public int Id_Pago_Proveedor { get; set; }
    public decimal Valor_Total { get; set; }
    public string Medio_Pago { get; set; } = string.Empty;
    public string Fecha_Pago { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int Id_Proveedor { get; set; }
    public int Id_Pedido { get; set; }

    public PagoProveedor ToEntidad()
    {
        var fecha = DateTime.TryParse(Fecha_Pago, out var parsedDate) ? parsedDate : DateTime.UtcNow;
        return PagoProveedor.Rehidratar(Id_Pago_Proveedor, Id_Proveedor, Id_Pedido, (double)Valor_Total, Medio_Pago, fecha, Estado);
    }
}
