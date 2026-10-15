namespace DistriFresasLY.Domain.Common;

/// <summary>
/// Helper central para manejar todas las fechas del sistema en la zona horaria
/// de Colombia (America/Bogota, UTC-5). Colombia no aplica horario de verano
/// desde 2013, por lo que el desfase es fijo todo el año.
/// <para>
/// Convención: en todo el sistema las fechas se almacenan y trabajan como
/// hora local de Colombia (DateTimeKind.Unspecified). Solo se convierte a UTC
/// en los límites estrictamente necesarios.
/// </para>
/// </summary>
public static class ColombiaTime
{
    private static readonly TimeZoneInfo ZonaColombia = ObtenerZonaColombia();

    /// <summary>
    /// Hora actual de Colombia (Kind = Unspecified, hora local de pared).
    /// Usar como valor por defecto de cualquier fecha generada por el sistema.
    /// </summary>
    public static DateTime Ahora => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ZonaColombia);

    /// <summary>
    /// Desfase de Colombia respecto a UTC (siempre -05:00).
    /// </summary>
    public static TimeSpan Offset => ZonaColombia.BaseUtcOffset;

    /// <summary>
    /// Normaliza una fecha recibida desde fuera para que quede como hora local de Colombia.
    /// <list type="bullet">
    /// <item>Kind = Utc se convierte a hora de Colombia.</item>
    /// <item>Kind = Local se convierte desde la zona del servidor a Colombia.</item>
    /// <item>Kind = Unspecified se asume que ya es hora de Colombia y no se toca.</item>
    /// </list>
    /// </summary>
    public static DateTime AColombia(DateTime valor)
    {
        return valor.Kind switch
        {
            DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(valor, ZonaColombia),
            DateTimeKind.Local => TimeZoneInfo.ConvertTime(valor, ZonaColombia),
            _ => valor
        };
    }

    /// <summary>
    /// Convierte una hora local de Colombia a UTC, útil cuando se requiere
    /// el instante absoluto (por ejemplo, para comparaciones o persistencia remota).
    /// </summary>
    public static DateTime AUtc(DateTime horaColombia)
    {
        var especificada = DateTime.SpecifyKind(horaColombia, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(especificada, ZonaColombia);
    }

    private static TimeZoneInfo ObtenerZonaColombia()
    {
        try
        {
            // En .NET 6+ el ID IANA funciona en Windows (vía ICU) y en Linux/macOS.
            return TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
        }
        catch (Exception)
        {
            // Fallback para plataformas sin resolución IANA.
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            }
            catch (Exception)
            {
                // Último recurso: desfase fijo de Colombia (UTC-5), sin horario de verano.
                return TimeZoneInfo.CreateCustomTimeZone(
                    "Colombia (UTC-5)",
                    TimeSpan.FromHours(-5),
                    "Colombia",
                    "Colombia");
            }
        }
    }
}
