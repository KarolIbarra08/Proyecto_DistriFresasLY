using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.ValueObjects;

public sealed record Calibre
{
    public int Value { get; }

    private Calibre(int value)
    {
        Value = value;
    }

    public static Result<Calibre> Create(int value)
    {
        if (value <= 0)
        {
            return Result.Failure<Calibre>(
                Error.Validation(
                    "Calibre.Invalido",
                    "El calibre debe ser mayor que cero."));
        }

        return Result.Success(new Calibre(value));
    }

    public static implicit operator int(Calibre calibre)
        => calibre.Value;
}