using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.ValueObjects;

public sealed record PrecioCompra
{
    public decimal Value { get; }

    private PrecioCompra(decimal value)
    {
        Value = value;
    }

    public static Result<PrecioCompra> Create(decimal value)
    {
        if (value <= 0)
        {
            return Result.Failure<PrecioCompra>(
                Error.Validation(
                    "PrecioCompra.Invalido",
                    "El precio de compra debe ser mayor que cero."));
        }

        return Result.Success(new PrecioCompra(value));
    }

    public static implicit operator decimal(PrecioCompra precio)
        => precio.Value;
}