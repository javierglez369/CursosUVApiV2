namespace Domain.Entities;

public class Leccion : BaseEntity
{
    public required string Titulo { get; set; }
    public string? VideoUrl { get; set; }
    public string? Contenido { get; set; }
    public int Orden { get; set; }
    public int DuracionMinutos { get; set; }
    public bool EsGratuita { get; set; }

    public int ModuloId { get; set; }
    public Modulo? Modulo { get; set; }

    public ICollection<Progreso> Progresos { get; set; } = [];
}