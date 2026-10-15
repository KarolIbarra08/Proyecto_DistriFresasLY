using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DistriFresasLY.Api.Endpoints;
using DistriFresasLY.Api.Middleware;
using DistriFresasLY.Domain.Common;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración para permitir parseo ultra flexible de fechas
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new CustomDateTimeConverter());
});

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("DistriFresasLY API")
               .WithTheme(ScalarTheme.Moon)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();


public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? dateString = reader.GetString();

            if (string.IsNullOrWhiteSpace(dateString))
                return default;

            // Parseamos preservando el Kind (Utc / Local / Unspecified) para saber
            // si la fecha trae offset "Z"/explícito o es hora local "desnuda".
            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateInvariant))
            {
                // Si venía como UTC ("Z") o Local, se convierte a hora de Colombia.
                // Si venía sin offset (Unspecified), se asume que ya es hora de Colombia.
                return ColombiaTime.AColombia(dateInvariant);
            }

            if (DateTime.TryParse(dateString, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind, out var dateLocal))
            {
                return ColombiaTime.AColombia(dateLocal);
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            // Timestamp Unix (ms) -> es UTC absoluto, se convierte a hora de Colombia.
            return ColombiaTime.AColombia(DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).UtcDateTime);
        }

        throw new JsonException("El valor de fecha ingresado no tiene un formato válido.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // El valor en memoria ya es hora de Colombia; se serializa como ISO 8601
        // con el desfase local de Colombia para que sea inequívoco (-05:00).
        var enColombia = ColombiaTime.AColombia(value);
        var conOffset = new DateTimeOffset(DateTime.SpecifyKind(enColombia, DateTimeKind.Unspecified), ColombiaTime.Offset);

        writer.WriteStringValue(conOffset.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture));
    }
}