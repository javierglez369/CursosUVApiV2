namespace Application.DTOs;

public record CreateCategoriaDto
{
    public required string Nombre { get; init; }
    public string? Descripcion { get; init; }
}