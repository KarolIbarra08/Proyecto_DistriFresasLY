using System.Text.RegularExpressions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.ValueObjects;

public sealed record CedulaNit
{
    private static readonly Regex Regex = new(
        @"^[0-9]{6,12}(-[0-9kK]{1})?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private CedulaNit(string value)
    {
        Value = value;
    }

    public static Result<CedulaNit> Create(string? documento)
    {
        if (string.IsNullOrWhiteSpace(documento))
        {
            return Result.Failure<CedulaNit>(Error.Validation(
                "CedulaNit.Requerida",
                "El numero de Cedula o NIT es obligatorio."));
        }

        var docLimpio = documento.Trim();

        if (!Regex.IsMatch(docLimpio))
        {
            return Result.Failure<CedulaNit>(Error.Validation(
                "CedulaNit.FormatoInvalido",
                "El formato de la Cédula o NIT no es valido."));
        }

        return Result.Success(new CedulaNit(docLimpio.ToUpperInvariant()));
    }

    public static implicit operator string(CedulaNit cedulaNit) => cedulaNit.Value;

    public override string ToString() => Value;
}