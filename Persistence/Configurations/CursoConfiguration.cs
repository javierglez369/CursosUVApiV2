using Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("Cursos");

        builder.Property(c => c.Titulo).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Descripcion).HasMaxLength(2000);
        builder.Property(c => c.Precio).HasColumnType("decimal(10,2)");
        builder.Property(c => c.Tags).HasMaxLength(500);

        // Enum almacenado como string (más legible en BD)
        builder.Property(c => c.Nivel)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Relaciones
        builder.HasOne(c => c.Categoria)
            .WithMany(cat => cat.Cursos)
            .HasForeignKey(c => c.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Instructor)
            .WithMany(i => i.Cursos)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
        [
            new Curso { Id = 1, Titulo = "ASP.NET Core desde Cero",      CategoriaId = 1, InstructorId = 1, Precio = 499.00m, Nivel = NivelCurso.Principiante, Estado = EstadoCurso.Publicado, CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) },
            new Curso { Id = 2, Titulo = "SQL Server Avanzado",           CategoriaId = 4, InstructorId = 2, Precio = 699.00m, Nivel = NivelCurso.Avanzado,     Estado = EstadoCurso.Publicado, CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) },
            new Curso { Id = 3, Titulo = "Diseño UX para Desarrolladores",CategoriaId = 2, InstructorId = 3, Precio = 399.00m, Nivel = NivelCurso.Intermedio,   Estado = EstadoCurso.Publicado, CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) },
            new Curso { Id = 4, Titulo = "Docker y Kubernetes",           CategoriaId = 5, InstructorId = 1, Precio = 799.00m, Nivel = NivelCurso.Avanzado,     Estado = EstadoCurso.Borrador,  CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) }
        ]);
    }
}