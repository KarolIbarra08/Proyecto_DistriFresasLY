using System.Text.RegularExpressions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.ValueObjects;


public sealed record CedulaNit
{
    // Acepta dígitos y un guion opcional para el dígito de verificación del NIT  900123456-1 o 34567890
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
            return Error.Validation(
                "CedulaNit.Requerida",
                "El número de Cédula o NIT es obligatorio.");
        }

        var docLimpio = documento.Trim();

        if (!Regex.IsMatch(docLimpio))
        {
            return Error.Validation(
                "CedulaNit.FormatoInvalido",
                "El formato de la Cédula o NIT no es válido.");
        }

        return new CedulaNit(docLimpio.ToUpperInvariant());
    }

    public static implicit operator string(CedulaNit cedulaNit) => cedulaNit.Value;

    public override string ToString() => Value;
}