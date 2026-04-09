namespace Domain.Entities;

public class Instructor : BaseEntity
{
    public required string Nombre { get; set; }
    public required string Apellidos { get; set; }
    public required string Email { get; set; }
    public string? Bio { get; set; }
    public string? FotoUrl { get; set; }
    public bool Activo { get; set; } = true;    
    public ICollection<Curso> Cursos { get; set; } = [];
}