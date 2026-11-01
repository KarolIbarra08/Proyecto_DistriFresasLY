using DistriFresasLY.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DistriFresasLY.Api.Middleware;

/// <summary>
/// Manejador global de excepciones no controladas según el estándar RFC 7807 (ProblemDetails).
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ConflictoException conflicto)
        {
            _logger.LogWarning(
                "Conflicto de negocio: {Message} | Ruta: {Path}",
                conflicto.Message,
                httpContext.Request.Path);

            var conflictoDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflicto de Estado",
                Detail = conflicto.Message,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                Instance = httpContext.Request.Path
            };

            conflictoDetails.Extensions["errorCode"] = conflicto.Code;
            conflictoDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

            await httpContext.Response.WriteAsJsonAsync(conflictoDetails, cancellationToken);

            return true;
        }

        _logger.LogError(
            exception,
            "Excepción no controlada capturada: {Message} | Ruta: {Path}",
            exception.Message,
            httpContext.Request.Path);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Error Interno del Servidor",
            Detail = "Ha ocurrido un error inesperado al procesar la solicitud.",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Instance = httpContext.Request.Path
        };

        // Agregar traceId para facilitar el rastreo y soporte
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}