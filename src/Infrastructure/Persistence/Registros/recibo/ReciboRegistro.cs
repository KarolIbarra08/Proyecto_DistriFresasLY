using DistriFresasLY.Domain.Entities.Recibos;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

public sealed class ReciboRegistro
{
    public int Id_Recibo { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public int Numero_Recibo { get; set; }
    public decimal Total { get; set; }
    public int Id_Venta { get; set; }

    public Recibo ToEntidad()
    {
        var fecha = DateTime.TryParse(Fecha, out var parsedDate) ? parsedDate : DateTime.UtcNow;
        return Recibo.Rehidratar(Id_Recibo, fecha, Numero_Recibo, (double)Total, Id_Venta);
    }
}
