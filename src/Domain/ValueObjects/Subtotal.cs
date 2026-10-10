using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.ValueObjects;

public sealed record Subtotal
{
    public decimal Value { get; }

    private Subtotal(decimal value)
    {
        Value = value;
    }

    public static Result<Subtotal> Create(decimal value)
    {
        if (value < 0)
        {
            return Result.Failure<Subtotal>(
                Error.Validation(
                    "Subtotal.Invalido",
                    "El subtotal no puede ser negativo."));
        }

        return Result.Success(new Subtotal(value));
    }

    public static implicit operator decimal(Subtotal subtotal)
        => subtotal.Value;
}