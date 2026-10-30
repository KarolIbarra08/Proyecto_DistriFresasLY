using DistriFresasLY.Domain.Entities.ProductosDanados;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

public class ProductoDanadoRegistro
{
    public int Id_Producto_Danado { get; set; }
    public int Id_Producto { get; set; }
    public string Tipo_Producto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public string Identificacion { get; set; } = string.Empty;
    public double Valor { get; set; }
    public string Fecha_Producto_Danado { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;

    public ProductoDanado ToEntidad()
    {
        DateTime.TryParse(Fecha_Producto_Danado, out var fecha);

        var result = ProductoDanado.Create(
            Id_Producto,
            Tipo_Producto,
            Cantidad,
            Identificacion,
            (decimal)Valor,
            fecha,
            Motivo,
            Id_Producto_Danado);

        return result.Value;
    }
}