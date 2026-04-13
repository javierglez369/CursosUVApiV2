using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Persistence.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    :IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Instructor> Instructores => Set<Instructor>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Modulo> Modulos => Set<Modulo>();
    public DbSet<Leccion> Lecciones => Set<Leccion>();
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<Inscripcion> Inscripciones => Set<Inscripcion>();
    public DbSet<Progreso> Progresos => Set<Progreso>();
    public DbSet<Resena> Resenas => Set<Resena>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<Categoria>().HasQueryFilter(c => !c.IsDeleted);

    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var ahora = DateTime.UtcNow;

        foreach(var entry in ChangeTracker.Entries<BaseEntity>())
        {

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt= ahora;
                    entry.Entity.UpdatedAt = ahora;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = ahora;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = ahora;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }


}
