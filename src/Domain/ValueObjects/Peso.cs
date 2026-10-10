using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.ValueObjects;

public sealed record Peso
{
    public double Value { get; }

    private Peso(double value)
    {
        Value = value;
    }

    public static Result<Peso> Create(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return Result.Failure<Peso>(
                Error.Validation(
                    "Peso.Invalido",
                    "El peso debe ser un valor numérico válido."));
        }

        if (value <= 0)
        {
            return Result.Failure<Peso>(
                Error.Validation(
                    "Peso.Invalido",
                    "El peso debe ser mayor que cero."));
        }

        return Result.Success(new Peso(value));
    }

    public static implicit operator double(Peso peso)
        => peso.Value;
}