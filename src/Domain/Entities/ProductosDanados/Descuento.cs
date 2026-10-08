using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.ProductosDanados;

public class Descuento
{
    public double Valor { get; private set; }
    public DateTime Fecha { get; private set; }
    public string Motivo { get; private set; }

    private Descuento(double valor, DateTime fecha, string motivo)
    {
        Valor = valor;
        Fecha = fecha;
        Motivo = motivo;
    }

    public static Result<Descuento> Create(double valor, DateTime fecha, string motivo)
    {
        if (valor < 0)
            return Result.Failure<Descuento>(Error.Validation("Descuento.ValorInvalido", "El valor del descuento no puede ser negativo."));

        return Result.Success(new Descuento(valor, fecha, motivo.Trim()));
    }
}