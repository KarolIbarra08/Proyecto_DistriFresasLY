using System.Text.RegularExpressions;

using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.ValueObjects;

public sealed record Telefono
{
    public string Valor { get; }

    private Telefono(string valor)
    {
        Valor = valor;
    }

    public static Result<Telefono> Create(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return Result.Failure<Telefono>(
                Error.Validation(
                    "Telefono.Obligatorio",
                    "El telefono es obligatorio."));
        }

        valor = valor.Trim();

        if (!Regex.IsMatch(valor, @"^\d{10}$"))
        {
            return Result.Failure<Telefono>(
                Error.Validation(
                    "Telefono.Invalido",
                    "El teléfono debe contener exactamente 10 dígitos."));
        }

        return Result.Success(new Telefono(valor));
    }
}