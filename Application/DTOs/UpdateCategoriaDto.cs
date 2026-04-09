namespace Application.DTOs;

public record UpdateCategoriaDto
{
    public required string Nombre { get; init; }
    public string? Descripcion { get; init; }
}

//public record AddUpdateCategoriaDto {
//    public required string Nombre { get; init; }
//    public string? Descripcion { get; init; }
//}