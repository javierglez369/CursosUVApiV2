namespace Domain.Entities;

public class Progreso : BaseEntity
{
    public int InscripcionId { get; set; }
    public int LeccionId { get; set; }
    public bool Completada { get; set; }
    public DateTime? FechaCompletado { get; set; }

    public Inscripcion? Inscripcion { get; set; }
    public Leccion? Leccion { get; set; }
}