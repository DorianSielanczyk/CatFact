using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CatFact.API.Exceptions
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Wystąpił nieoczekiwany błąd: {Message}", exception.Message);

            var (statusCode, title) = exception switch
            {
                HttpRequestException or TimeoutException =>
                    (StatusCodes.Status503ServiceUnavailable, "Zewnętrzne API jest chwilowo niedostępne."),

                UnauthorizedAccessException or IOException =>
                    (StatusCodes.Status500InternalServerError, "Wystąpił błąd podczas zapisu danych na serwerze."),

                System.Text.Json.JsonException =>
                    (StatusCodes.Status502BadGateway, "Otrzymano nieprawidłową odpowiedź z zewnętrznego API."),

                TaskCanceledException or OperationCanceledException =>
                    (499, "Żądanie zostało anulowane."),

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

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
