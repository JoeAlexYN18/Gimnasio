using System.Net;
using System.Text.Json;
using Gimnasio.DTOs;

namespace Gimnasio.Middleware
{
    /// <summary>
    /// Middleware de manejo centralizado de excepciones.
    /// Intercepta cualquier excepción no controlada y la convierte
    /// en una respuesta JSON con el formato estándar <see cref="ApiResponse{T}"/>.
    ///
    /// Mapa de excepciones a códigos HTTP:
    /// <list type="table">
    ///   <item><see cref="KeyNotFoundException"/>       → 404 Not Found</item>
    ///   <item><see cref="UnauthorizedAccessException"/> → 401 Unauthorized</item>
    ///   <item><see cref="InvalidOperationException"/>  → 400 Bad Request</item>
    ///   <item><see cref="ArgumentException"/>          → 400 Bad Request (reglas de dominio)</item>
    ///   <item>Cualquier otra excepción                → 500 Internal Server Error</item>
    /// </list>
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next   = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción no controlada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                KeyNotFoundException          => (HttpStatusCode.NotFound,           exception.Message),
                UnauthorizedAccessException   => (HttpStatusCode.Unauthorized,        exception.Message),
                InvalidOperationException     => (HttpStatusCode.BadRequest,          exception.Message),
                ArgumentException             => (HttpStatusCode.BadRequest,          exception.Message), 
                _                             => (HttpStatusCode.InternalServerError, "Ocurrió un error interno en el servidor.")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)statusCode;

            var response = ApiResponse<object>.Fail(message);
            var json     = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}