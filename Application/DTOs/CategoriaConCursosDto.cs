namespace Application.DTOs;

public record CategoriaConCursosDto(
    int Id,
    string Nombre,
    string? Descripcion,
    bool Activa,
    IEnumerable<CursoResumenDto> Cursos
    );