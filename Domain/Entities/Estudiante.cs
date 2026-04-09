namespace Domain.Entities;

public class Estudiante : BaseEntity
{
    public required string Nombre { get; set; }
    public required string Apellidos { get; set; }
    public required string Email { get; set; }
    public string? FotoUrl { get; set; }

    // Vinculado al usuario de Identity (Día 3)
    public string? UserId { get; set; }
    public ICollection<Inscripcion> Inscripciones { get; set; } = [];
}