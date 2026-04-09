namespace Application.DTOs;

public record UpdateCategoriaDto
{
    public required string Nombre { get; init; }
    public string? Descripcion { get; init; }
    public bool Activa { get; init; }
}