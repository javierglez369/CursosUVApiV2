using Domain.Enums;

namespace Domain.Entities;

public class Curso : BaseEntity
{
    public required string Titulo { get; set; }
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public NivelCurso Nivel { get; set; } = NivelCurso.Principiante;
    public EstadoCurso Estado { get; set; } = EstadoCurso.Borrador;
    public string? Tags { get; set; }
    public string? ImagenUrl { get; set; }

    // FK
    public int CategoriaId { get; set; }
    public int InstructorId { get; set; }

    // Para autorización basada en recursos (Día 3)
    public string? CreadoPorId { get; set; }

    // Navigation properties
    public Categoria? Categoria { get; set; }
    public Instructor? Instructor { get; set; }
    public ICollection<Modulo> Modulos { get; set; } = [];
    public ICollection<Inscripcion> Inscripciones { get; set; } = [];
    public ICollection<Resena> Resenas { get; set; } = [];
}