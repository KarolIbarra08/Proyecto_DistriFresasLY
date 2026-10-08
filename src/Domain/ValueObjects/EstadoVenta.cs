using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.ValueObjects;

public sealed record EstadoVenta
{
    public static readonly EstadoVenta Pendiente = new("Pendiente");
    public static readonly EstadoVenta Completada = new("Completada");
    public static readonly EstadoVenta Cancelada = new("Cancelada");

    private static readonly string[] EstadosValidos = [Pendiente.Value, Completada.Value, Cancelada.Value];

    public string Value { get; }

    private EstadoVenta(string value)
    {
        Value = value;
    }

    public static Result<EstadoVenta> Create(string? estado)
    {
        if (string.IsNullOrWhiteSpace(estado))
        {
            return Result.Failure<EstadoVenta>(Error.Validation(
                "EstadoVenta.Requerido",
                "El estado de la venta es obligatorio."));
        }

        var estadoNormalizado = estado.Trim();
        var coincide = EstadosValidos.FirstOrDefault(e => e.Equals(estadoNormalizado, StringComparison.OrdinalIgnoreCase));

        if (coincide is null)
        {
            return Result.Failure<EstadoVenta>(Error.Validation(
                "EstadoVenta.Invalido",
                $"El estado '{estado}' no es valido. Estados permitidos: {string.Join(", ", EstadosValidos)}."));
        }

        return Result.Success(new EstadoVenta(coincide));
    }

    public static implicit operator string(EstadoVenta estado) => estado.Value;

    public override string ToString() => Value;
}