namespace Domain.Entities;

public class Categoria:BaseEntity
{
    public required string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool Activa { get; set; }

    // ✅ Navigation property — colección, no List
    public ICollection<Curso> Cursos { get; set; } = [];

}
