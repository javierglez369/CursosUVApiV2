using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ProgresoConfiguration : IEntityTypeConfiguration<Progreso>
{
    public void Configure(EntityTypeBuilder<Progreso> builder)
    {
        builder.ToTable("Progresos");

        // Relación con Inscripcion: Cascade (registro de progreso depende de la inscripción)
        builder.HasOne(p => p.Inscripcion)
            .WithMany()
            .HasForeignKey(p => p.InscripcionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con Leccion: NoAction (evita ciclos de cascada)
        // Curso → Modulos → Lecciones → Progresos
        // Curso → Inscripciones → Progresos
        builder.HasOne(p => p.Leccion)
            .WithMany()
            .HasForeignKey(p => p.LeccionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}