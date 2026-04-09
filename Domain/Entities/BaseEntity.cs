namespace Domain.Entities;

//BaseEntity - Clase Base para todas las entidades del dominio.
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    //Soft delete: nunca borramos físicamente de la BD
    public bool IsDeleted { get; set; }

}
