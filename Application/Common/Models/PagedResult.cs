namespace Application.Common.Models;

public record PagedResult<T>(
    IEnumerable<T> Data,
    int TotalRecords,
    int PageNumber,
    int PageSize
)
{
    // Propiedad calculada: cuántas páginas existen en total
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}