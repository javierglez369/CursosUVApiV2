namespace Application.DTOs;

public record CategoriaExisteDto(
    bool Existe,
    int? Id,
    string? Nombre
    );
