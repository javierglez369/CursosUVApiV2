namespace Domain.Entities;

public class Estudiante : BaseEntity
{
    public required string Nombre { get; set; }
    public required string Apellidos { get; set; }
    public required string Email { get; set; }
    public string? FotoUrl { get; set; }

    // Vinculado al usuario de Identity (Día 3)

    // ── Vínculo con Identity ──────────────────────────────────────────────
    // UserId es el Id de ApplicationUser (IdentityUser.Id — string/Guid)
    public string? UserId { get; set; }

    // Navegación inversa — se configura en Persistence
    public ApplicationUser? User { get; set; }

    public ICollection<Inscripcion> Inscripciones { get; set; } = [];
    public ICollection<Resena> Resenas { get; set; } = [];
}