namespace Application.DTOs;

public record CreateCategoriaConCursosDto
{
    public required string Nombre { get; init; }
    public string? Descripcion { get; init; }
    public IList<CreateCursoEnCategoriaDto> Cursos { get; init; } = [];
}
