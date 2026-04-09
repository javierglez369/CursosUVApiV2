namespace Domain.Entities;

public class Modulo : BaseEntity
{
    public required string Titulo { get; set; }
    public int Orden { get; set; }
    public int DuracionMinutos { get; set; }
    public int CursoId { get; set; }
    public Curso? Curso { get; set; }
    public ICollection<Leccion> Lecciones { get; set; } = [];
    
}