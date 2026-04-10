using Domain.Enums;

namespace Application.DTOs;

public record CreateCursoEnCategoriaDto
{
    public required string Titulo { get; init; }
    public required string Descripcion { get; init; }
    public decimal Precio { get; init; }
    public NivelCurso Nivel { get; init; } = NivelCurso.Principiante;
}
