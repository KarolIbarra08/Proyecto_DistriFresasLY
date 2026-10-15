using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.ValueObjects;

public sealed record Descuento
{
    public decimal Valor { get; }
    public DateTime Fecha { get; }
    public string Motivo { get; }

    private Descuento(
        decimal valor,
        DateTime fecha,
        string motivo)
    {
        Valor = valor;
        Fecha = fecha;
        Motivo = motivo;
    }

    public static Result<Descuento> CalcularValor(
        int cantidad,
        decimal valorUnitario,
        string motivo)
    {
        if (cantidad <= 0)
        {
            return Result.Failure<Descuento>(
                Error.Validation(
                    "Descuento.CantidadInvalida",
                    "La cantidad debe ser mayor que cero."));
        }

        if (valorUnitario <= 0)
        {
            return Result.Failure<Descuento>(
                Error.Validation(
                    "Descuento.ValorInvalido",
                    "El valor debe ser mayor que cero."));
        }

        if (string.IsNullOrWhiteSpace(motivo))
        {
            return Result.Failure<Descuento>(
                Error.Validation(
                    "Descuento.MotivoRequerido",
                    "El motivo del descuento es obligatorio."));
        }

        var valorDescuento = cantidad * valorUnitario;

        return Result.Success(
            new Descuento(
                valorDescuento,
                ColombiaTime.Ahora,
                motivo.Trim()));
    }
}