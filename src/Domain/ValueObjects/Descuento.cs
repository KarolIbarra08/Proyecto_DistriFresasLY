using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.ValueObjects;

public record Descuento
{
    public double Valor { get; }
    public DateTime Fecha { get; }
    public string Motivo { get; }

    private Descuento(double valor, DateTime fecha, string motivo)
    {
        Valor = valor;
        Fecha = fecha;
        Motivo = motivo;
    }

    public static Result<Descuento> Create(double valor, DateTime fecha, string motivo)
    {
        if (valor < 0)
        {
            return Result.Failure<Descuento>(Error.Validation(
                "Descuento.ValorInvalido", 
                "El valor del descuento no puede ser negativo."));
        }

        if (string.IsNullOrWhiteSpace(motivo))
        {
            return Result.Failure<Descuento>(Error.Validation(
                "Descuento.MotivoRequerido", 
                "El motivo del descuento es requerido."));
        }

        return Result.Success(new Descuento(valor, fecha, motivo.Trim()));
    }


    public static Result<Descuento> CalcularValor(int cantidad, double valorUnitario, string motivo)
    {
        if (cantidad <= 0)
        {
            return Result.Failure<Descuento>(Error.Validation(
                "Descuento.CantidadInvalida", 
                "La cantidad debe ser mayor a cero para calcular el descuento."));
        }

        double valorTotalDescuento = cantidad * valorUnitario;

        return Create(
            valorTotalDescuento, 
            DateTime.UtcNow, 
            $"Descuento por producto dañado: {motivo}"
        );
    }
}