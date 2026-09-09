using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Text.Json;

namespace CatFact.API.Exceptions
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
        {
            if (exception is OperationCanceledException)
            {
                logger.LogInformation("Żądanie zostało anulowane przez klienta (Client Disconnected).");

                return true;
            }

            logger.LogError(exception, "Wystąpił nieoczekiwany błąd: {Message}", exception.Message);

            var (statusCode, title) = exception switch
            {
                HttpRequestException or TimeoutException or TimeoutRejectedException =>
                   (StatusCodes.Status503ServiceUnavailable, "Zewnętrzne API jest chwilowo niedostępne."),

                BrokenCircuitException =>
                    (StatusCodes.Status503ServiceUnavailable, "Zewnętrzne API jest tymczasowo wyłączone z obsługi (zbyt wiele błędów)."),

                JsonException =>
                    (StatusCodes.Status502BadGateway, "Otrzymano nieprawidłową odpowiedź z zewnętrznego API."),

                UnauthorizedAccessException or IOException =>
                    (StatusCodes.Status500InternalServerError, "Wystąpił błąd podczas zapisu danych na serwerze."),

                ArgumentNullException or ArgumentException =>
                    (StatusCodes.Status400BadRequest, "Nieprawidłowa konfiguracja lub parametry żądania."),

                _ =>
                    (StatusCodes.Status500InternalServerError, "Wystąpił wewnętrzny błąd serwera.")
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
            };

            httpContext.Response.StatusCode = statusCode;

            if (!httpContext.Response.HasStarted)
            {
                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            }

            return true;
        }
    }
}
