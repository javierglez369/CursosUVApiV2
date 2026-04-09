namespace Application.Common.Models;

public class QueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        // Math.Clamp garantiza que nunca sea < 1 ni > 100
        set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }

    public string? SearchTerm { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDirection { get; set; } = "asc";

    // C# 13 — collection expressions para valores por defecto
    public string[] SearchFields { get; set; } = [];
    public string[] Includes { get; set; } = [];
}