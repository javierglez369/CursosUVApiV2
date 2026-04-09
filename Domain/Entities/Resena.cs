namespace Domain.Entities;

public class Resena : BaseEntity
{
    public int EstudianteId { get; set; }
    public Estudiante? Estudiante { get; set; }
    public int CursoId { get; set; }
    public Curso? Curso { get; set; }
    public int Calificacion { get; set; } // 1 a 5
    public string? Comentario { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;    
}