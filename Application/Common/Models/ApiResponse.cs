namespace Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }   
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public int? StatusCode { get; set; }
    public string? TraceId { get; set; }  // Para correlación de logs

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        => new() { 
            Success = true, 
            Data = data, 
            Message = message,
            StatusCode = 200
        };

    public static ApiResponse<T> ErrorResponse(
        string message,
        List<string>? errors = null,
        int statusCode = 400,
        string? traceId = null)
    => new()
    {
        Success = false,
        Message = message,
        Errors = errors,
        StatusCode = statusCode,
        TraceId = traceId
    };

}
