using System.Net;

namespace Domain.Exceptions;

/// <summary>
/// Excepción de negocio con código HTTP incorporado.
/// Se usa para representar errores que el dominio puede anticipar y nombrar.
/// </summary>
public class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public List<string> Errors { get; }

    // Constructor principal
    public AppException(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = [];  // ✅ C# 13 collection expression
    }

    // Constructor con lista de errores detallados
    public AppException(
        string message,
        List<string> errors,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors ?? [];
    }

    // Constructor con excepción interna (para wrapping)
    public AppException(
        string message,
        Exception innerException,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        Errors = [];
    }

    // ── Métodos estáticos de conveniencia ─────────────────────────────────────
    public static AppException NotFound(string message)
        => new(message, HttpStatusCode.NotFound);

    public static AppException BadRequest(string message)
        => new(message, HttpStatusCode.BadRequest);

    public static AppException BadRequest(string message, List<string> errors)
        => new(message, errors, HttpStatusCode.BadRequest);

    public static AppException Forbidden(string message)
        => new(message, HttpStatusCode.Forbidden);

    public static AppException Unauthorized(string message)
        => new(message, HttpStatusCode.Unauthorized);
}