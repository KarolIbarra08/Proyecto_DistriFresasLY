using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.ValueObjects;

public sealed record Cantidad
{
    public int Value { get; }

    private Cantidad(int value)
    {
        Value = value;
    }

    public static Result<Cantidad> Create(int value)
    {
        if (value <= 0)
        {
            return Result.Failure<Cantidad>(
                Error.Validation(
                    "Cantidad.Invalida",
                    "La cantidad inicial debe ser mayor que cero."));
        }

        return Result.Success(
            new Cantidad(value));
    }

    public static implicit operator int(Cantidad cantidad)
    {
        return cantidad.Value;
    }
}