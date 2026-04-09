using Application.Common.Models;
using Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Api.Middleware;

/// <summary>
/// Middleware de manejo global de excepciones.
/// Convierte cualquier excepción no capturada en una respuesta ApiResponse estandarizada.
/// DEBE registrarse ANTES de UseRouting en Program.cs.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly bool _isDevelopment;

    // ✅ Constructor tradicional: _isDevelopment se calcula UNA VEZ en construcción.
    // Primary constructor no sirve aquí porque necesitamos computar _isDevelopment
    // a partir de env antes de asignarlo, y env no se necesita después.
    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _isDevelopment = env.IsDevelopment();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // ✅ Switch expression (C# 13) con property pattern para AppException
        var statusCode = exception switch
        {
            AppException { StatusCode: var sc } => sc,           // property pattern
            ArgumentException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        // Log diferenciado: WARNING para errores de negocio, ERROR para inesperados
        if (exception is AppException)
        {
            _logger.LogWarning(
                "Business Error: {Message} | StatusCode: {StatusCode} | TraceId: {TraceId} | Path: {Path}",
                exception.Message, (int)statusCode, context.TraceIdentifier, context.Request.Path);
        }
        else
        {
            _logger.LogError(exception,
                "Unhandled Exception: {Type} | Message: {Message} | StatusCode: {StatusCode} | TraceId: {TraceId} | Path: {Path}",
                exception.GetType().Name, exception.Message, (int)statusCode,
                context.TraceIdentifier, context.Request.Path);
        }

        // Construir respuesta según entorno
        ApiResponse<object> response;

        if (_isDevelopment)
        {
            List<string> errors = [];   // ✅ collection expression C# 13

            // Si la AppException trae errores detallados, incluirlos
            if (exception is AppException appExDev && appExDev.Errors.Count > 0)
                errors.AddRange(appExDev.Errors);

            // En desarrollo, agregar StackTrace como último error
            if (exception.StackTrace is not null)           // ✅ is not null
                errors.Add($"StackTrace: {exception.StackTrace}");

            response = ApiResponse<object>.ErrorResponse(
                exception.Message,
                errors.Count > 0 ? errors : null,
                (int)statusCode,
                context.TraceIdentifier);
        }
        else
        {
            // En producción, solo el mensaje si es AppException; genérico para el resto
            var message = exception switch
            {
                AppException appEx => appEx.Message,
                ArgumentException => "Solicitud inválida.",
                UnauthorizedAccessException => "No autorizado.",
                _ => "Ha ocurrido un error interno. Contacte al soporte técnico."
            };

            var errors = exception is AppException appExProd && appExProd.Errors.Count > 0
                ? appExProd.Errors
                : null;

            response = ApiResponse<object>.ErrorResponse(
                message, errors, (int)statusCode, context.TraceIdentifier);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _isDevelopment
        });

        await context.Response.WriteAsync(json);
    }
}