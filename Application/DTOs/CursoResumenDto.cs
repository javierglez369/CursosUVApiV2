namespace Application.DTOs;

public record CursoResumenDto(
    int Id,
    string Titulo,
    string Estado,
    decimal Precio
    );