using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("Instructores");

        builder.Property(i => i.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Apellidos).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Email).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Bio).HasMaxLength(1000);

        // Índice único en Email
        builder.HasIndex(i => i.Email).IsUnique();

        builder.HasData(
        [
            new Instructor { Id = 1, Nombre = "Ana",    Apellidos = "García",   Email = "ana.garcia@cursosUV.com",    Bio = "Experta en .NET y arquitectura de software", Activo = true, CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) },
            new Instructor { Id = 2, Nombre = "Carlos", Apellidos = "López",    Email = "carlos.lopez@cursosUV.com",  Bio = "Especialista en bases de datos y SQL Server", Activo = true, CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) },
            new Instructor { Id = 3, Nombre = "María",  Apellidos = "Martínez", Email = "maria.martinez@cursosUV.com", Bio = "Diseñadora UX con 10 años de experiencia",    Activo = true, CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) }
        ]);
    }
}
