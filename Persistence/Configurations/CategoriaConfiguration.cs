using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Descripcion)
            .HasMaxLength(500);

        // ✅ Seed data con HasData
        builder.HasData(
        [
            new Categoria { Id = 1, Nombre = "Programación",   Descripcion = "Cursos de desarrollo de software", Activa = true, CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) },
            new Categoria { Id = 2, Nombre = "Diseño",         Descripcion = "Cursos de diseño gráfico y UX",     Activa = true, CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) },
            new Categoria { Id = 3, Nombre = "Marketing",      Descripcion = "Cursos de marketing digital",       Activa = true, CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) },
            new Categoria { Id = 4, Nombre = "Bases de Datos", Descripcion = "SQL, NoSQL y modelado de datos",    Activa = true, CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) },
            new Categoria { Id = 5, Nombre = "DevOps",         Descripcion = "CI/CD, Docker, Kubernetes",         Activa = true, CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) }
        ]);
    }
}