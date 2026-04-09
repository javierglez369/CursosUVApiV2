using Domain.Enums;

namespace Domain.Entities;

public class Inscripcion : BaseEntity
{
    public int EstudianteId { get; set; }
    public int CursoId { get; set; }
    public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
    public EstadoInscripcion Estado { get; set; } = EstadoInscripcion.Pendiente;
    public decimal MontoPagado { get; set; }
    public Estudiante? Estudiante { get; set; }
    public Curso? Curso { get; set; }
    public ICollection<Progreso> Progresos { get; set; } = [];
}
